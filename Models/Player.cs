namespace TicTacToe.Models;

public class Player
{
    public Guid Guid { get; set; }
    public string Nickname { get; set; }
    
    public int AccountId { get; set; }
    public Account? Account { get; set; }
}