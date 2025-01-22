using Microsoft.AspNetCore.Mvc;
using MockAuthProvider.Contracts;
using MockAuthProvider.Services.Interfaces;

namespace MockAuthProvider.Controllers
{
    [ApiController]
    public class AuthenticationController: ControllerBase
    {
        private readonly IClientsService _clientsService;

        public AuthenticationController(IClientsService clientsService)
            => _clientsService = clientsService;

        [HttpPost]
        [Route("/authorize")]
        public IActionResult Authorize([FromBody] AuthorizeRequestContract request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var authorized = _clientsService.Authorize(request);

            if (!authorized.Success)
            {
                return Unauthorized(authorized.ErrorMessage);
            }

            return Ok(authorized);
        }
    }
}