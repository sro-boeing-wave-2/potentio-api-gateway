using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserLoginAPI.Models;

namespace UserLoginAPI.Services
{
    public class UsersControllerService : IUsersControllerService
    {
        private readonly UserLoginAPIContext _context;

        private readonly string Salt = "gj+oAMieIg+2B/eoxA31+w==";
        private readonly byte[] Saltbyte;

        public UsersControllerService(UserLoginAPIContext context)
        {
            _context = context;
            Saltbyte = Encoding.ASCII.GetBytes(Salt);
        }

        public IEnumerable<User> GetUserService()
        {
            return _context.User;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task<User> PutUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> PostUser([FromBody] User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return null;
            }
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<string> Login(string email, string password)
        {
            bool dbEmail = EmailExists(email);

            if (dbEmail)
            {
                User user = await GetUserByEmail(email);
                string hashpassword = HashPassword(password);
                if (hashpassword == user.Password)
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@007"));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    var tokenOptions = new JwtSecurityToken(
                        issuer: "https://localhost:44397",
                        audience: "https://locahost:44397",
                        claims: new List<Claim> {
                            new Claim("UserID", user.UserID.ToString()),
                            new Claim("FirstName", user.FirstName),
                            new Claim("LastName", user.LastName),
                            new Claim("Email", user.Email),
                        },
                        expires: DateTime.Now.AddHours(2),
                        signingCredentials: signinCredentials
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                    return tokenString;
                }
                else
                {
                    return "Incorrect Password";
                }
            }
            else
            {
                return null;
            }
        }

        public bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserID == id);
        }

        public bool EmailExists(string email)
        {
            return _context.User.Any(x => x.Email == email);
        }

        public string HashPassword(string Password)
        {
            string hashpassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: Password,
            salt: Saltbyte,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
            return hashpassword;
        }

        public bool EmailChanged(int id, string email)
        {
            var user = _context.User.AsNoTracking().SingleOrDefault(x => x.UserID == id);
            if (user.Email != email)
            {
                return true;
            }
            return false;
        }

        public string GetUserIDfromToken(string Token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokens = handler.ReadJwtToken(Token);
            var userid = tokens.Claims.First(cl => cl.Type == "UserID").Value;
            return userid;
        }

    }

    public interface IUsersControllerService
    {
        IEnumerable<User> GetUserService();
        Task<User> GetUser(int id);
        Task<User> GetUserByEmail(string email);
        Task<User> PutUser(User user);
        Task<User> PostUser(User user);
        Task<User> DeleteUser(int id);
        Task<string> Login(string email, string password);
        bool UserExists(int id);
        bool EmailExists(string email);
        string HashPassword(string Password);
        bool EmailChanged(int id, string email);
        string GetUserIDfromToken(string Token);
    }
}
