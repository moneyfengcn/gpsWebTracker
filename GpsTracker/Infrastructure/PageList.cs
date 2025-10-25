using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Infrastructure
{
    public class PageList<T>
    {
        public int Total { get; }

        public List<T> Items { get; }

        public int PageSize { get; }

        public int PageIndex { get; }

        public int TotalPage { get; }

        public bool HasPrev => PageIndex > 1;

        public bool HasNext => PageIndex < TotalPage;

        public PageList(int total, int pageSize, int pageIndex, List<T> items)
        {
            this.Total = total;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.Items = items;
            TotalPage = ((this.Total % this.PageSize == 0) ? (this.Total / this.PageSize) : (this.Total / this.PageSize + 1));
        }
    }
}
