using System;

namespace Api.Application.Domain.Dtos.User
{
    public class UserDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }

        public UserDto(string id, string name, string userName)
        {
            ID = Guid.Parse(id);
            Name = name;
            UserName = userName;
        }

        public UserDto()
        {

        }
    }
}
