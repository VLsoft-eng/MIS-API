using System.IdentityModel.Tokens.Jwt;

namespace Api.Extensions;

public static class HttpContextExtension
{
    public static string? GetUserEmail(this HttpContext httpContext)
    {
        if (httpContext.User.Identity == null || httpContext.User.Identity.IsAuthenticated == false)
        {
            return null;
        }

        return httpContext.User.FindFirst("doctorEmail")?.Value;
    }

    public static string? GetUserId(this HttpContext httpContext)
    {
        if (httpContext.User.Identity == null || httpContext.User.Identity.IsAuthenticated == false)
        {
            return null;
        }

        return httpContext.User.FindFirst("doctorId")?.Value;
    }

    public static string? GetTokenId(this HttpContext httpContext)
    {
        if (httpContext.User.Identity == null || httpContext.User.Identity.IsAuthenticated == false)
        {
            return null;
        }

        return httpContext.User.FindFirst("tokenId")?.Value;
    }
    
    public static string? GetJwtToken(this HttpContext httpContext)
    {
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        return null; 
    }
    
    public static DateTime? GetTokenExpiration(this HttpContext httpContext)
    {
        var token = httpContext.GetJwtToken();

        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var handler = new JwtSecurityTokenHandler();

        if (handler.CanReadToken(token))
        {
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo; 
        }

        return null;
    }
}