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
}