using System.Linq;
using WebApi.Responses;
using Domain.Responses;

namespace WebApi.Helpers
{
    public static class WebPaginationHelper<T> where T : class
    {
        private const string PageParameter = "?page=";
        private const string PageSizeParameter = "&pageSize=";

        public static WebPaginatedResponse<T> GenerateWebPaginatedResponse(PaginatedResponse<T> paginatedResponse, int page, int pageSize, string route)
        {
            return new WebPaginatedResponse<T>
            {
                TotalElements = paginatedResponse.TotalElements,
                TotalPages = paginatedResponse.TotalPages,
                Results = paginatedResponse.Elements,
                CurrentPageItems = paginatedResponse.Elements.Count(),
                CurrentPageNumber = page,
                CurrentPageUrl = route + PageParameter + page + PageSizeParameter + pageSize,
                PreviousPageUrl =
                    page == 1
                        ? string.Empty
                        : route + PageParameter + (page - 1) + PageSizeParameter + pageSize,
                NextPageUrl =
                    page == paginatedResponse.TotalPages
                        ? string.Empty
                        : route + PageParameter + (page + 1) + PageSizeParameter + pageSize
            };
        }
    }
}