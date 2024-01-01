namespace TicTacToe.Models;

public class Room
{
    public Guid Guid { get; set; }
    public string RoomName { get; set; }
    public int MaxPlayerCount { get; set; }
    public List<Player> Players { get; set; }
    public Dictionary<Player, bool> ReadyDict { get; set; }
    public TicTacToe Game { get; set; }

    public bool TryJoinRoom(Player player)
    {
        if (Players.Count == MaxPlayerCount)
        {
            return false;
        }

        if (Players.Any(x => x.Guid == player.Guid))
        {
            return false;
        }
        
        Players.Add(player);
        return true;
    }

    public void ToggleReady(Player player)
    {
        if (!Players.Contains(player))
        {
            throw new InvalidOperationException();
        }

        if (ReadyDict.TryGetValue(player, out var previousValue))
        {
            ReadyDict[player] = !previousValue;
        }
        else
        {
            ReadyDict[player] = true;
        }

        if (Players.Count == MaxPlayerCount && Players.All(p => ReadyDict.ContainsKey(p) && ReadyDict[p]))
        {
            Game.StartGame(Players[0], Players[1]);
        }
    }
}