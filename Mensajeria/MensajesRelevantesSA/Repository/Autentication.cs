using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel;
using System.Security;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MensajesRelevantesSA.Repository
{
    public class Autentication
    {
        string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090" +
                     "fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
        public string GenerateToken()
        {
            var securityKey = new Microsoft.IdentityModel.Tokens.
                SymmetricSecurityKey(Encoding.Default.GetBytes(key));

            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                  (securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload
            {
               { "some ", "hello "},
               { "scope", "http://dummy.com/"},
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

    }
}