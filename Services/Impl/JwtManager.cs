using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TicTacToe.Services.Impl;

public class JwtManager : IJwtManager
{
    private readonly IConfiguration _configuration;

    public JwtManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateJwtToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        var jwtToken = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return token;
    }
}