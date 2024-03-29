﻿using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthServices
    {
        private readonly IRepository<User> _user;
        private readonly IConfiguration _configuration;

        public AuthServices(IRepository<User> user, IConfiguration configuration)
        {
            _user = user;
            _configuration = configuration;
        }

        public async Task<String> RegisterUser(String data)
        {
            string paswrdHash = BCrypt.Net.BCrypt.HashPassword(data);
            data= paswrdHash;
            //await _user.Add(data);
            return data;
        }


        //Checking the validity of user-----------------------------------------------------
        public async Task<string> CheckUser(UserLog data)
        {
            var tmp =await _user.Get(data.UserId);
            if(tmp != null) 
            {
                if (BCrypt.Net.BCrypt.Verify(data.Password, tmp.Password))
                {
                    return "Valid";
                }
                else
                {
                    return "Invalid Password";
                }
            }
            else
            {
                return "Invalid User Id";
            }

        }


        //JWT token issuing------------------------------------------------------------
        public async Task<string> CreateToken(int UserId)
        {
            var tmp = await _user.Get(UserId);

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,tmp.Id.ToString()),
                new Claim(ClaimTypes.Name, tmp.Name),
                new Claim(ClaimTypes.Role, tmp.Role),
                new Claim("IssuedAt", DateTime.UtcNow.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!.PadRight(64, '\0')));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
