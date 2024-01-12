using System.Security.Claims;

namespace TicTacToe.Services;

public interface IJwtManager
{
    public string CreateJwtToken(IEnumerable<Claim> claims);
}