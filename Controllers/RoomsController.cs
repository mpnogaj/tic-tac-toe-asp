using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Dto;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRoomManager _roomManager;
    
    public RoomsController(IMapper mapper, IRoomManager roomManager)
    {
        _mapper = mapper;
        _roomManager = roomManager;
    }

    [HttpGet]
    public IActionResult GetRooms()
    {
        return Ok(_roomManager.Rooms.Select(x => _mapper.Map<RoomDto>(x)).ToList());
    }

    [HttpGet("{guid}")]
    public IActionResult GetRoom(Guid guid)
    {
        var room = _roomManager.GetRoom(guid);

        if (room == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<RoomDto>(room));
    }

    [HttpPost]
    public IActionResult CreateRoom([FromBody]RoomDto roomDto)
    {
        var creator = new Player()
        {
            Guid = User.GetGuid(),
            Nickname = User.GetNickname(),
            Account = null
        };

        var room = _roomManager.AddRoom(roomDto.RoomName, roomDto.MaxPlayerCount, creator);

        return Ok(_mapper.Map<RoomDto>(room));
    }

    [HttpPost("{guid}")]
    public IActionResult JoinRoom(Guid guid)
    {
        var room = _roomManager.GetRoom(guid);

        if (room == null)
        {
            return NotFound();
        }
        
        var player = new Player
        {
            Guid = User.GetGuid(),
            Nickname = User.GetNickname(),
            Account = null,
        };

        if (room.TryJoinRoom(player))
        {
            return Ok(guid);   
        }
        else
        {
            return Forbid();
        }
    }
}