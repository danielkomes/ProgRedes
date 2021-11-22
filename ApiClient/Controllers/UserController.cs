using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IClientService clientService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserController(
            IClientService clientService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.clientService = clientService;
        }
        [HttpGet]
        public async Task<ActionResult<WebPaginatedResponse<Client>>> GetClientsAsync(int page = 1, int pageSize = 15)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest();
            }
            PaginatedResponse<Client> clientPaginatedResponse =
                await clientService.GetClients(page, pageSize);
            if (clientPaginatedResponse.Elements == null)
            {
                return NoContent();
            }

            string route = httpContextAccessor.HttpContext.Request.Host.Value +
                           httpContextAccessor.HttpContext.Request.Path;
            WebPaginatedResponse<Client> response =
                WebPaginationHelper<Client>.GenerateWebPaginatedResponse(clientPaginatedResponse, page, pageSize, route);

            return Ok(response);
        }
    }
}
