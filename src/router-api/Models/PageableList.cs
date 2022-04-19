using System.Collections.Generic;

namespace RouterApi.Models
{
    public class PageableList<T>
        where T : class
    {
        public PageableList()
        {
            Data = new List<T>();
        }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public List<T> Data { get; set; }
    }
}
