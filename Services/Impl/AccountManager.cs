using Microsoft.EntityFrameworkCore;
using TicTacToe.Db;
using TicTacToe.Models;

namespace TicTacToe.Services.Impl;

public class AccountManager : IAccountManager
{
    private readonly AppDbContext _dbContext;

    public AccountManager(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account?> GetAccountInfo(Guid playerGuid)
    {
        var player = await _dbContext.Players
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Guid == playerGuid);
        return player?.Account;
    }

    public async Task IncreaseGamesPlayed(Guid playerGuid)
    {
        var account = await GetAccountInfo(playerGuid);
        if (account == null) return;
        account.MatchesPlayed++;
        await _dbContext.SaveChangesAsync();
    }

    public async Task IncreaseGamesWon(Guid playerGuid)
    {
        var account = await GetAccountInfo(playerGuid);
        if (account == null) return;
        account.MatchesWon++;
        await _dbContext.SaveChangesAsync();
    }
}