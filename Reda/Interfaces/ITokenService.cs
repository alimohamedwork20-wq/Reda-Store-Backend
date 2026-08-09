using Reda.Entities;

namespace Reda.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
