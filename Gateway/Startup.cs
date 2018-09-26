using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Logging;
using Ocelot.Provider.Consul;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Consul;
using Chilkat;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Gateway
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Console.WriteLine(env.ContentRootPath);
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddJsonFile("configuration.json");

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var provider in Configuration.Providers)
            {
                Console.WriteLine(provider.GetType());
            }
            services.AddCors(o => o.AddPolicy("AppPolicy", builder =>
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowAnyOrigin()
                   .AllowCredentials()
                )
            );
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOcelot(Configuration).AddConsul();
            services.AddSignalR();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseHsts();

            //}
            app.UseCors("AppPolicy");
            app.Use(async (context, next) =>
            {


                Console.WriteLine(context.Request.Path);
                //if (context.Request.Query.TryGetValue("access_token", out var token1))
                //{
                //    context.Request.Headers.Add("Authorization", $"Bearer {token1}");
                //}
                if (context.Request.Path == "/auth/login" || context.Request.Path == "/auth/register" ||
                context.Request.Path == "/auth" || context.Request.Path == "/auth/email" ||
                context.Request.Path == "/auth/logout" || context.Request.Path == "/auth/details" || context.Request.Path == "/home" || context.Request.Path == "/dashboard" )
                {
                    Console.WriteLine("Calling next middleware");
                    await next();
                }

                //if (_context.Request.Query.TryGetValue("access_token", out var token))
                //{
                //    _context.Request.Headers.Add("Authorization", $"Bearer {token}");
                //}
                //await next.Invoke();

                Chilkat.Global global = new Chilkat.Global();
                global.UnlockBundle("Anything for 30-day trail");
                Chilkat.Jwt jwt = new Chilkat.Jwt();

                using (var client = new ConsulClient())
                {
                    string ConsulIP = Environment.GetEnvironmentVariable("MACHINE_LOCAL_IP4");
                    client.Config.Address = new Uri(Environment.GetEnvironmentVariable("MACHINE_LOCAL_IP4") + ":8500");
                    //client.Config.Address = new Uri("http://172.17.0.1:8500");
                    var getPair = await client.KV.Get("secretkey");
                    string token = context.Request.Cookies["UserLoginAPItoken"];
                    //string token = context.Request.Headers["Authorization"];

                    Console.WriteLine(token + "\n");
                    if (token != null)
                    {
                        var x = token.Replace("Bearer ", "");
                        Console.WriteLine(x + "\n");
                        Rsa rsaPublicKey = new Rsa();
                        rsaPublicKey.ImportPublicKey(Encoding.UTF8.GetString(getPair.Response.Value));

                        Console.WriteLine(Encoding.UTF8.GetString(getPair.Response.Value) + "\n");

                        var isTokenVerified = jwt.VerifyJwtPk(x, rsaPublicKey.ExportPublicKeyObj());

                        Console.WriteLine(rsaPublicKey.ExportPublicKeyObj() + "\n");

                        Console.WriteLine(isTokenVerified + "\n");
                        if (isTokenVerified)
                        {
                            Console.WriteLine(isTokenVerified + "\n");
                            await next();
                        }
                        else
                        {
                            await context.Response.WriteAsync("Unauthorised");
                        }
                    }
                }
            });
            // app.UseOcelot().Wait();
            //app.Use(async (context, next) => { Console.WriteLine("Next middleware"); await next(); });
            app.UseOcelot().Wait();
        }
    }
}