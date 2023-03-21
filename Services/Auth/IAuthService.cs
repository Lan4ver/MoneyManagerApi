using MoneyManagerApi.Models.API;

namespace MoneyManagerApi.Services.Auth
{
    public interface IAuthService
    {
        string GetToken(long userId);
    }
}
