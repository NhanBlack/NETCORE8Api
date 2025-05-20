using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

public class CustomAuthorizeFilter : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public CustomAuthorizeFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var headers = context.HttpContext.Request.Headers;

        if (!headers.ContainsKey("Authorization"))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var authHeader = headers["Authorization"].ToString();

        if (!authHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var secretKey = _configuration["Jwt:Key"];

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Bạn có thể gán ClaimsPrincipal vào HttpContext.User nếu cần
            context.HttpContext.User = principal;

            await next();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            context.Result = new UnauthorizedResult();
        }
    }
}
