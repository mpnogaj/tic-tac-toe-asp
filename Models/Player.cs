namespace TicTacToe.Models;

public class Player
{
    public Guid Guid { get; set; }
    public string Nickname { get; set; }
    public bool IsLoggedIn { get; set; }
    public Account? Account { get; set; }
}