using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Security;

/// <summary>
/// JWT token validator for API Gateway
/// </summary>
public class JwtValidator
{
    private readonly IConfiguration _configuration;

    public JwtValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool ValidateToken(string token)
    {
        var jwtOptions = _configuration.GetSection("Jwt").Get<JwtOptions>() 
            ?? throw new InvalidOperationException("JWT configuration is missing");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}

