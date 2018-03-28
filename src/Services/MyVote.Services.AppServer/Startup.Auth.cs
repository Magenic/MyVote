using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace MyVote.Services.AppServer
{
    public partial class Startup
    {

        /// <summary>
        /// Sets up JWT middleware configuration for use when authorizing endpoints within this API
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureAuth(IServiceCollection services)
        {

            //Todo: place in configuration
            var signingKey = new SymmetricSecurityKey(FromHex("Enter_your_WEBSITE_AUTH_SIGNING_KEY_here"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
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
                    ValidIssuer = "https://mbl-myapp.azurewebsites.net/",

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = "https://mbl-myapp.azurewebsites.net/",

                    // Validate the token expiry
                    ValidateLifetime = false,

                    // If you want to allow a certain amount of clock drift, set that here:
                    ClockSkew = TimeSpan.Zero
                };
                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();

                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync("An error occurred processing your authentication.");
                    }
                };
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
