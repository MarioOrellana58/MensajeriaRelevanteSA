using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel;
using System.Security;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using MensajesRelevantesSA.Models;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MensajesRelevantesSA.Repository
{
    class JWTNode
    {
        public string Username { get; set; }
        public string MacAdress { get; set; }
    }
    public class Autentication
    {


        public async Task<string> GenerateJWT(string loggedUser)
        {
            string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
                  string addr = "";
            foreach (NetworkInterface mac in NetworkInterface.GetAllNetworkInterfaces()) {
                if (mac.OperationalStatus == OperationalStatus.Up) {
                    addr = mac.GetPhysicalAddress().ToString();
                    break;
                }
            }
            
            var jwtData = new JWTNode()
            {
                Username = loggedUser,
                MacAdress = addr
            };

            var json = JsonConvert.SerializeObject(jwtData);//data
            var token = await GenerarJWT(loggedUser, loggedUser, key, json);
            await Console.Out.WriteLineAsync(token);

            return token;
        }


        public async Task<string> GenerarJWT(string loggedUser, string loggedUser2, string key, string json)       
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = await CreateClaimsIdentities(json);

            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: loggedUser,
                audience: loggedUser2,
                subject: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(10),
                signingCredentials:
                new SigningCredentials(new SymmetricSecurityKey(Encoding.Default.GetBytes(key)),SecurityAlgorithms.HmacSha256Signature));

            return tokenHandler.WriteToken(token);
        }

        public static Task<ClaimsIdentity> CreateClaimsIdentities(string userJson)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, userJson));
            return Task.FromResult(claimsIdentity);
        }

    }
}