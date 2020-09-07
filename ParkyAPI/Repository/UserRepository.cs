using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public UserRepository(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
            
        }

        public User Authenticate(string username, string password)
        {
            var user = _db.Users.Where(x => x.Username == username && x.Password == password).SingleOrDefault();

            //user not found
            if (user == null)
            {
                return null;
            }

            var secret = _config["AppSettings:Secret"];

            //if user was found generate JWT Token
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenhandler.CreateToken(tokendescriptor);
            user.Token = tokenhandler.WriteToken(token);
            user.Password = "";
            return user;
        }

        public bool isUniqueUser(string username)
        {
            var user = _db.Users.Where(x => x.Username.ToUpper() == username.ToUpper());

            if (user == null)
            {
                return true;
            }

            return false;
        }

        public User Register(string username, string password)
        {
            User userObj = new User()
            {
                Username = username,
                Password = password,
                Role = "Admin"
            };

            _db.Users.Add(userObj);
            _db.SaveChanges();
            userObj.Password = "";

            return userObj;
        }
    }
}
