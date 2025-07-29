using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Pagination
{
    public class PaginationRequest
    {
        private const int _maxPageSize = 25;
        private int _pageSize = 10;
        public int PageSize { get => _pageSize; set => _pageSize = value > _maxPageSize ? _maxPageSize : value; }
        public int PageNumber { get; set; } = 1;
    }

}
