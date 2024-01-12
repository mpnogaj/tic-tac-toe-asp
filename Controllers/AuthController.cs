using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Db;
using TicTacToe.Dto;
using TicTacToe.Models;
using TicTacToe.Services;
using BC = BCrypt.Net.BCrypt;

namespace TicTacToe.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtManager _jwtManager;
    private readonly IMapper _mapper;
    
    public AuthController(AppDbContext dbContext, IJwtManager jwtManager, IMapper mapper)
    {
        _dbContext = dbContext;
        _jwtManager = jwtManager;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var user = await _dbContext.Accounts
            .Include(x => x.Player)
            .FirstOrDefaultAsync(x => x.Username == loginModel.Username);

        if (user == null)
        {
            return Unauthorized();
        }

        if (!BC.Verify(loginModel.Password, user.PasswordHash))
        {
            return Unauthorized();
        }

        var token = _jwtManager.CreateJwtToken(new List<Claim>
        {
            new(ClaimTypes.Name, user.Player.Nickname),
            new Claim(ClaimTypes.Authentication, true.ToString(), ClaimValueTypes.Boolean),
            new(ClaimTypes.NameIdentifier, user.Player.Guid.ToString())
        });
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
        });
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
    {
        var playerGuid = Guid.NewGuid();

        var player = new Player
        {
            Guid = playerGuid,
            Nickname = registerModel.Nickname
        };
        
        var account = new Account()
        {
            Id = 0,
            Username = registerModel.Username,
            PasswordHash = BC.HashPassword(registerModel.Password),
            MatchesPlayed = 0,
            MatchesWon = 0,
            Player = player
        };

        await _dbContext.Players.AddAsync(player);
        await _dbContext.Accounts.AddAsync(account);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    [Authorize("LoggedOnly")]
    public async Task<IActionResult> GetProfile()
    {
        var player = await GetPlayerFromClaims(User.Claims);
        if (player?.Account == null) return Unauthorized();

        return Ok(_mapper.Map<AccountDto>(player.Account));
    }

    [HttpGet("ping")]
    [Authorize("LoggedOnly")]
    public IActionResult Ping()
    {
        return Ok();
    }

    [HttpPatch]
    [Authorize("LoggedOnly")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateModel updateModel)
    {
        var player = await GetPlayerFromClaims(User.Claims);

        if (player == null) return Unauthorized();

        if (!BC.Verify(updateModel.OldPassword, player.Account!.PasswordHash)) return Unauthorized();
        
        player.Nickname = updateModel.Nickname;
        player.Account.PasswordHash = BC.HashPassword(updateModel.NewPassword);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    private async Task<Player?> GetPlayerFromClaims(IEnumerable<Claim> claims)
    {
        var guidClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (guidClaim == null) return null;

        var parseSuccessful = Guid.TryParse(guidClaim.Value, out var guid);

        if (!parseSuccessful) return null;

        var player = await _dbContext.Players
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Guid == guid);

        return player;
    }
}

public record LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public record RegisterModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Nickname { get; set; }
}

public record UpdateModel
{
    public string Nickname { get; set; }
    public string NewPassword { get; set; }
    public string OldPassword { get; set; }
}