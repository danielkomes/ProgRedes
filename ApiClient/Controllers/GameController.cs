using WebApi.Helpers;
using WebApi.Responses;
using WebApi.Services;
using Domain;
using Domain.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult<WebPaginatedResponse<Game>>> GetGamesAsync(int page = 1, int pageSize = 15)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest();
            }
            PaginatedResponse<Game> gamesPaginatedResponse =
                await gameService.GetGames(page, pageSize);
            if (gamesPaginatedResponse == null)
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
            Game game = await gameService.GetGameById(id);
            if (game != null)
            {
                return Ok(game);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Game>> PublishGameAsync(Game game)
        {
            var responseGame = await gameService.PublishGameAsync(game);
            return new CreatedResult(string.Empty, responseGame);
        }

        //[HttpPut]
        //public async Task<ActionResult<Student>> UpdateStudentAsync(Student student)
        //{
        //    var responseStudent = await _studentService.UpdateStudentAsync(student);
        //    return Ok(responseStudent);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStudentAsync(int id)
        //{
        //    var student = await _studentService.GetStudentByIdAsync(id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    await _studentService.DeleteStudentAsync(student);
        //    return NoContent();
        //}
    }
}
