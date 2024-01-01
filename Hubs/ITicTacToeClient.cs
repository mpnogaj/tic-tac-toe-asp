using TicTacToe.Dto;

namespace TicTacToe.Hubs;

public interface ITicTacToeClient
{
    public Task NotifyPlayerJoined(PlayerDto player);
    public Task NotifyPlayerLeft(string playerGuid);
    public Task NotifyToggleReady(string playerGuid, bool ready);
    public Task NotifyGameStarted(PlayerDto firstPlayer);
    public Task UpdateBoardState(string board, PlayerDto nextMove);
    public Task NotifyGameFinished(string? winner);
}