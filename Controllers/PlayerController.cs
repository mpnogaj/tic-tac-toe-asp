using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TicTacToe.Dto;

namespace TicTacToe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PlayerController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public IActionResult LoginAnonymously([FromBody]PlayerDto playerDto)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, playerDto.Nickname),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        var jwtToken = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
        });
        return Ok(token);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok(User.GetNickname());
    }
}