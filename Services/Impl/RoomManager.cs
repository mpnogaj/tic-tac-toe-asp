using System.Collections.ObjectModel;
using TicTacToe.Dto;
using TicTacToe.Models;

namespace TicTacToe.Services.Impl;

public class RoomManager : IRoomManager
{
    private readonly List<Room> _rooms = new();
    public ReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();

    public Room? GetRoom(Guid guid)
    {
        return _rooms.FirstOrDefault(x => x.Guid == guid);
    }

    public Room AddRoom(string roomName, int maxPlayers, Player creator)
    { 
        var room = new Room()
        {
            Guid = Guid.NewGuid(),
            RoomName = roomName,
            Players = new List<Player>
            {
                creator
            },
            MaxPlayerCount = maxPlayers,
            ReadyDict = new Dictionary<Player, bool>(),
            Game = new Models.TicTacToe()
        };

        _rooms.Add(room);

        return room;
    }

    public void LeaveRoom(Room room, Player player)
    {
        room.Players.Remove(player);
        room.ReadyDict.Remove(player);
        if (room.Players.Count == 0)
        {
            CloseRoom(room);
        }
    }

    public void CloseRoom(Room room)
    {
        if (_rooms.Contains(room))
        {
            _rooms.Remove(room);
        }
    }
}