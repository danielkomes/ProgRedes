using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Helpers;
using Domain.Responses;

namespace WebApi.Interfaces
{
    public interface IGameService
    {
        IEnumerable<Game> GetGames();
        Task<PaginatedResponse<Game>> GetGames(int page, int pageSize);
        Task<Game> GetGameByIdAsync(int id);
        Task<Game> PublishGameAsync(Game game);
        Task<Game> UpdateGameAsync(int id, Game game);
        Task<bool> DeleteGameAsync(int id);
        Task<PaginatedResponse<Review>> GetReviews(int id, int page, int pageSize);
    }
}