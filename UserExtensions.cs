using System.Security.Claims;

namespace TicTacToe;

public static class UserExtensions
{
    public static string GetNickname(this ClaimsPrincipal user)
    {
        return user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)!.Value;
    }

    public static Guid GetGuid(this ClaimsPrincipal user)
    {
        var guidString = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
        return Guid.Parse(guidString);
    }

    public static bool HasAccount(this ClaimsPrincipal user)
    {
        return user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email) != null;
    }
}