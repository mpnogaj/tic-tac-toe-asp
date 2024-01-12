namespace TicTacToe.Models;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    
    public Player Player { get; set; }
}