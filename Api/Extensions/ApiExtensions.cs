using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Abstractions.Repository;
using Domain;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

public static class ApiExtensions
{
    public static void AddApiAuthentication(
        this IServiceCollection services, 
        IOptions<JwtOptions> jwtOptions
       )
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey((Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey)))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenRepository = context.HttpContext.RequestServices.GetRequiredService<ITokenRepository>();
                        var tokenIdClaim = context.Principal.FindFirst("tokenId")?.Value;
                        var token = await tokenRepository.GetById(Guid.Parse(tokenIdClaim));

                        if (token != null)
                        {
                            context.Fail("sorry");
                        }
                    }
                };
            });

        services.AddAuthorization();
    }
}