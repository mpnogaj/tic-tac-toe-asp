namespace TicTacToe.Dto;

public class RoomDto
{
    public Guid Guid { get; set; }
    public string RoomName { get; set; }
    public int MaxPlayerCount { get; set; }
    public List<PlayerDto> Players { get; set; }
    public Dictionary<Guid, bool> ReadyDict { get; set; }
}