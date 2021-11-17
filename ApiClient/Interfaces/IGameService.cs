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
        PaginatedResponse<Game> GetGames(int page, int pageSize);
        Game GetGameById(int id);
        //Game SaveGameAsync(Game game);
        //Game UpdateGameAsync(Game game);
        //void DeleteGameAsync(Game game);
    }
}