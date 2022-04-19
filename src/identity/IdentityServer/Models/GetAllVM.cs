using System.Collections.Generic;

namespace IdentityServer.Models
{
    public class GetAllVM<T>
    {
        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public List<T> Data { get; set; }
    }
}
