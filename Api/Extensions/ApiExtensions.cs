using System.Text;
using Application.Abstractions.Repository;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

public static class ApiExtensions
{
    public static void AddApiAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration
       )
    {
        var jwtOptions = new JwtOptions();
        configuration.GetSection("JwtOptions").Bind(jwtOptions);
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey((Encoding.UTF8.GetBytes(jwtOptions.SecretKey)))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenRepository = context.HttpContext.RequestServices.GetRequiredService<ITokenRepository>();
                        var tokenIdClaim = context.Principal!.FindFirst("tokenId")?.Value;
                        var token = await tokenRepository.GetById(Guid.Parse(tokenIdClaim!));

                        if (token != null)
                        {
                            context.Fail("Token was banned.");
                        }
                    }
                };
            });

        services.AddAuthorization();
    }
}