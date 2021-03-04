using Api.Application.Domain.Dtos.Login;

namespace API.PP.Domain.Interfaces
{
    public interface IRepository
    {
        bool FindByLogin(LoginDto login);
    }
}
