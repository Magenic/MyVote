using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;

namespace MyVote.Services.AppServer
{
    public partial class Startup
    {

        /// <summary>
        /// Sets up JWT middleware configuration for use when authorizing endpoints within this API
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureAuth(IApplicationBuilder app)
        {

            //Todo: place in configuration
            var signingKey = new SymmetricSecurityKey(FromHex("<your key>"));            

            var tokenValidationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                SaveSigninToken = false,
                ValidateActor = false,

                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
               
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "https://mbl-myvote.azurewebsites.net/",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "https://mbl-myvote.azurewebsites.net/",

                // Validate the token expiry
                ValidateLifetime = false,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

        }

        /// <summary>
        /// Decodes a Hex string
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>byte[]</returns>
        private static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

    }
}
