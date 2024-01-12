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

        //if player is already on the list just don't add him
        if (Players.All(x => x.Guid != player.Guid))
        {
            Players.Add(player);
        }
        
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