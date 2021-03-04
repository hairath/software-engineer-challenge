using Api.Application.Domain.Dtos.Login;
using API.PP.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace API.PP.Data.Repository
{
    public class Repository : IRepository
    {
        private IConfiguration _configuration { get; }

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool FindByLogin(LoginDto login)
        {
            var userDefault = _configuration.GetValue<string>("AppIdentitySettings:User:UserDefault");
            var pwdDefault = _configuration.GetValue<string>("AppIdentitySettings:Password:PwdDefault");

            return login.UserName.ToLower() == userDefault && login.Password == pwdDefault;
        }
    }
}
