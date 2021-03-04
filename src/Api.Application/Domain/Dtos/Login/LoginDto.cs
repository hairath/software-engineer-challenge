using System.ComponentModel;

namespace Api.Application.Domain.Dtos.Login
{
    public class LoginDto
    {
        [DefaultValue("UserSystem")]
        public string UserName { get; set; }

        [DefaultValue("pwd123")]
        public string Password { get; set; }
    }
}
