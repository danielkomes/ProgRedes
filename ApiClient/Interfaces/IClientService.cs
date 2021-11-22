using Domain;
using Pagination;
using System.Threading.Tasks;

namespace WebApi.Interfaces
{
    public interface IClientService
    {
        Task<PaginatedResponse<Client>> GetClients(int page, int pageSize);
    }
}
