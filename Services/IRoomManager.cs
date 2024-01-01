using System.Collections.ObjectModel;
using TicTacToe.Models;

namespace TicTacToe.Services;

public interface IRoomManager
{
    public ReadOnlyCollection<Room> Rooms { get; }
    public Room? GetRoom(Guid guid);
    public Room AddRoom(string roomName, int maxPlayers, Player creator);
    public void LeaveRoom(Room room, Player player);
    public void CloseRoom(Room room);
}