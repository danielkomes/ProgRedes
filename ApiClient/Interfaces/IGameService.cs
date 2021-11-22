using Domain;
using Pagination;
using System.Threading.Tasks;

namespace WebApi.Interfaces
{
    public interface IGameService
    {
        Task<PaginatedResponse<Game>> GetGames(string title, string genre, int rating, int page, int pageSize);
        Task<Game> GetGameByIdAsync(int id);
        Task<Game> PublishGameAsync(Game game);
        Task<Game> UpdateGameAsync(int id, Game game);
        Task<bool> DeleteGameAsync(int id);
        Task<PaginatedResponse<Review>> GetReviews(int id, int page, int pageSize);
        Task<Review> ReviewGameAsync(int id, Review review);
    }
}