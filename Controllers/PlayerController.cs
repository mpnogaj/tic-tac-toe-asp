using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Dto;
using TicTacToe.Services;

namespace TicTacToe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IJwtManager _jwtManager;

    public PlayerController(IJwtManager jwtManager)
    {
        _jwtManager = jwtManager;
    }

    [HttpPost]
    public IActionResult LoginAnonymously([FromBody]PlayerDto playerDto)
    {
        var token = _jwtManager.CreateJwtToken(new List<Claim>
        {
            new(ClaimTypes.Name, playerDto.Nickname),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        });
        
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

    [HttpGet("ping")]
    [Authorize]
    public IActionResult Ping()
    {
        return Ok();
    }
}