﻿using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pagination;
using System.Threading.Tasks;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Route("games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService gameService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GameController(
            IGameService gameService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<WebPaginatedResponse<Game>>> GetGamesAsync(string title = "", string genre = "", int rating = -1, int page = 1, int pageSize = 15)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest();
            }
            PaginatedResponse<Game> gamesPaginatedResponse =
                await gameService.GetGames(title, genre, rating, page, pageSize);
            if (gamesPaginatedResponse.Elements == null)
            {
                return NoContent();
            }

            string route = httpContextAccessor.HttpContext.Request.Host.Value +
                           httpContextAccessor.HttpContext.Request.Path;
            WebPaginatedResponse<Game> response =
                WebPaginationHelper<Game>.GenerateWebPaginatedResponse(gamesPaginatedResponse, page, pageSize, route);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGameById(int id)
        {
            Game game = await gameService.GetGameByIdAsync(id);
            if (game != null)
            {
                return Ok(game);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Game>> PublishGameAsync(Game game)
        {
            Game responseGame = await gameService.PublishGameAsync(game);
            return new CreatedResult(responseGame.Id.ToString(), responseGame);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Game>> UpdateGameAsync(int id, Game game)
        {
            Game responseGame = await gameService.UpdateGameAsync(id, game);
            if (responseGame != null)
            {
                return Ok(responseGame);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameAsync(int id)
        {
            bool result = await gameService.DeleteGameAsync(id);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<WebPaginatedResponse<Game>>> GetReviewsAsync(int id, int page = 1, int pageSize = 15)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest();
            }
            Game game = await gameService.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            PaginatedResponse<Review> reviewsPaginatedResponse =
                await gameService.GetReviews(id, page, pageSize);
            if (reviewsPaginatedResponse == null)
            {
                return NoContent();
            }

            string route = httpContextAccessor.HttpContext.Request.Host.Value +
                           httpContextAccessor.HttpContext.Request.Path;
            WebPaginatedResponse<Review> response =
                WebPaginationHelper<Review>.GenerateWebPaginatedResponse(reviewsPaginatedResponse, page, pageSize, route);

            return Ok(response);
        }
        [HttpPost("{id}/reviews")]
        public async Task<ActionResult<Game>> PublishReviewAsync(int id, Review review)
        {
            Review responseReview = await gameService.ReviewGameAsync(id, review);
            if (responseReview != null)
            {
                return new CreatedResult(id + "/reviews", responseReview);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
