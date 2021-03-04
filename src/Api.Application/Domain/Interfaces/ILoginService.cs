using Api.Application.Domain.Dtos.Login;

namespace API.PP.Domain.Interfaces
{
    public interface ILoginService
    {
        object Authenticate(LoginDto user);
    }
}
