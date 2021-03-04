using Api.Application.Domain.Dtos.User;
using System.Collections.Generic;

namespace API.PP.Domain.Interfaces
{
    public interface IUserService
    {
        List<UserDto> Get(UserDTOFilter filter);
    }
}
