using Api.Application.Domain.Const;
using System.ComponentModel;

namespace Api.Application.Domain.Dtos.User
{
    public class UserDTOFilter
    {
        public const int MaxPageSize = SystemConstants.MaxPageSize;

        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;
        private int pageSize = SystemConstants.PageNumber;

        [DefaultValue(15)]
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public string Keyword { get; set; }
    }
}
