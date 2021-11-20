using System.Collections.Generic;

namespace Pagination
{
    public class WebPaginatedResponse<T> where T : class
    {
        public IEnumerable<T> Results { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public int CurrentPageNumber { get; set; }
        public int CurrentPageItems { get; set; }
        public string CurrentPageUrl { get; set; }
        public string PreviousPageUrl { get; set; }
        public string NextPageUrl { get; set; }
    }
}