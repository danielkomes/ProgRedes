using System;
using System.Collections.Generic;

namespace Pagination
{
    public static class PaginationHelper<T> where T : class
    {
        public static PaginatedResponse<T> GeneratePaginatedResponse(int pageSize, int totalElements, IEnumerable<T> elements)
        {
            return new PaginatedResponse<T>
            {
                Elements = elements,
                TotalElements = totalElements,
                TotalPages = (int)Math.Ceiling((double)totalElements / pageSize)
            };
        }
    }
}