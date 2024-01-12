using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using TicTacToe.Dto;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Hubs;

public class TicTacToeHub : Hub<ITicTacToeClient>
{
    private readonly IRoomManager _roomManager;
    private readonly IAccountManager _accountManager;
    private readonly IMapper _mapper;

    public TicTacToeHub(IRoomManager roomManager, IAccountManager accountManager, IMapper mapper)
    {
        _accountManager = accountManager;
        _roomManager = roomManager;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        var room = GetConnectionRoom(GetRoomGuidString());
        var player = GetConnectionPlayer(room);
        
        if (room == null || player == null)
        {
            Context.Abort();
            return;
        }

        await this.Groups.AddToGroupAsync(Context.ConnectionId, room.Guid.ToString());

        await Clients.GroupExcept(room.Guid.ToString(), Context.ConnectionId)
            .NotifyPlayerJoined(_mapper.Map<PlayerDto>(player));
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var room = GetConnectionRoom(GetRoomGuidString());
        var player = GetConnectionPlayer(room);
        
        if (room == null || player == null)
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        await Clients.GroupExcept(room.Guid.ToString(), Context.ConnectionId).NotifyPlayerLeft(player.Guid.ToString());

        if (room.Game.GameStarted)
        {
            room.Game.Surrender(player);
            if (room.Game.Winner != null)
            {
                await _accountManager.IncreaseGamesWon(room.Game.Winner.Guid);
            }
            
            await GetGroup(room).NotifyGameFinished(room.Game.Winner?.Nickname);
            _roomManager.CloseRoom(room);
        }

        _roomManager.LeaveRoom(room, player);
        
        await this.Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Guid.ToString());
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task ToggleReady()
    {
        var room = GetConnectionRoom(GetRoomGuidString());
        var player = GetConnectionPlayer(room);
        if (room == null || player == null)
        {
            return;
        }
        
        room.ToggleReady(player);
        await GetGroup(room).NotifyToggleReady(player.Guid.ToString(), room.ReadyDict[player]);

        if (room.Game.GameStarted)
        {
            foreach (var roomPlayer in room.Players)
            {
                await _accountManager.IncreaseGamesPlayed(roomPlayer.Guid);
            }
            await GetGroup(room).NotifyGameStarted(_mapper.Map<PlayerDto>(room.Game.CurrentTurn));
        }
    }

    public async Task MakeMove(int i, int j)
    {
        var room = GetConnectionRoom(GetRoomGuidString());
        var player = GetConnectionPlayer(room);
        if (room == null || player == null)
        {
            return;
        }

        var game = room.Game;
        
        if (!game.CanMakeMove(i, j, player))
        {
            return;
        }
        
        game.MakeMove(i, j, player);
        
        
        await GetGroup(room).UpdateBoardState(game.BoardToString(), _mapper.Map<PlayerDto>(game.CurrentTurn));
        if (game.GameFinished)
        {
            if (game.Winner != null)
            {
                await _accountManager.IncreaseGamesWon(game.Winner.Guid);
            }
            await GetGroup(room).NotifyGameFinished(game.Winner?.Nickname ?? null);
            _roomManager.CloseRoom(room);
        }
    }

    /// <summary>
    /// Gets reference to Player object in given room, and with user guid
    /// If user is not in the room function will return null
    /// </summary>
    /// <returns></returns>
    private Player? GetConnectionPlayer(Room? room)
    {
        if (Context.UserIdentifier == null)
        {
            return null;
        }

        if (!Guid.TryParse(Context.UserIdentifier, out var userGuid))
        {
            return null;
        }
        
        if (room == null)
        {
            return null;
        }

        var player = room.Players.FirstOrDefault(x => x.Guid == userGuid);
        return player;
    }
    
    private Room? GetConnectionRoom(string? roomGuidStr)
    {
        if (roomGuidStr == null)
        {
            return null;
        }

        if (!Guid.TryParse(roomGuidStr, out var roomGuid))
        {
            return null;
        }

        var room = _roomManager.GetRoom(roomGuid);
        return room;
    }
    
    private string? GetRoomGuidString()
    {
        var httpContext = Context.GetHttpContext();

        if (httpContext == null)
        {
            return null;
        }
        
        if (!httpContext.Request.Query.TryGetValue("room", out var values))
        {
            return null;
        }
        var roomGuidStr = values[0];
        
        return roomGuidStr ?? null;
    }

    private ITicTacToeClient GetGroup(Room room)
    {
        return Clients.Group(room.Guid.ToString());
    }
}