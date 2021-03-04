using Api.Application.Domain.Dtos.User;
using API.PP.Domain.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Api.Application.Services
{
    public class UserService : IUserService
    {
        public List<UserDto> Get(UserDTOFilter filter)
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            List<string> orderByList = new List<string>();

            //Read files 
            var dbFile = System.IO.File.ReadLines(Path.Combine(assemblyFolder, "Data/users.csv")).Select(x => x.Split(','));
            var filePriorityFirst = System.IO.File.ReadLines(Path.Combine(assemblyFolder, "Data/lista_relevancia_1.txt")).ToList();
            var filePrioritySecond = System.IO.File.ReadLines(Path.Combine(assemblyFolder, "Data/lista_relevancia_2.txt")).ToList();

            orderByList.AddRange(filePriorityFirst);
            orderByList.AddRange(filePrioritySecond);

            int i = 0;
            var filterList = dbFile.AsParallel()
                                   .Where(x => (string.IsNullOrEmpty(filter.Keyword) || (x[1].ToLower().Contains(filter.Keyword.ToLower()) || x[2].ToLower().Contains(filter.Keyword.ToLower()))))
                                   .OrderBy(x => (i = orderByList.IndexOf(x[0])) < 0 ? int.MaxValue : i)
                                   .Skip((filter.PageNumber - 1) * filter.PageSize)
                                   .Take(filter.PageSize);

            var userList = new List<UserDto>();

            foreach (var user in filterList)
                userList.Add(new UserDto(user[0], user[1], user[2]));

            return userList;
        }
    }
}
