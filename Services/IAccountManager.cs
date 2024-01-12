using TicTacToe.Models;

namespace TicTacToe.Services;

public interface IAccountManager
{
    public Task<Account?> GetAccountInfo(Guid userGuid);
    public Task IncreaseGamesPlayed(Guid userGuid);
    public Task IncreaseGamesWon(Guid userGuid);
}