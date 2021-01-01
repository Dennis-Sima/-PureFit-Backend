using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using PureFit_REST.api.Services;
using PureFit_REST.Dto;
using PureFit_REST.api.Model;
using System.Linq;

namespace PureFit_REST.api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly PureFitDbContext _context;
        private readonly AuthService _authService;

        public UserController(PureFitDbContext context, AuthService authService)
        {
            _authService = authService;
            this._context = context;
            
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> RegisterKundenUser(UserKundenDto userKunde)
        {          
              await  _authService.CreateUserKunde(userKunde, _context);         
              return Ok(userKunde);
        }
        [HttpPost("register2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserKundenDto>> Register2Async([FromBody]UserKundenDto userKunde)
        {
            try
            {
                await _authService.CreateUserKunde(userKunde, _context);
                return Ok(userKunde);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = e.Message, Details = e.InnerException?.Message });
            }
        }



        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Login(UserDto user)
        {
            string token = await _authService.GenerateToken(user, TimeSpan.FromHours(3), _context);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        [HttpPost("login2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> Login2Async([FromBody]UserDto user)
        {
            try
            {
               string token = await _authService.GenerateToken(user, TimeSpan.FromHours(3), _context);

                // HTTP 401 liefern, wenn der User nicht authentifiziert werden kann.
                if (token == null)
                    return Unauthorized();
                user.Token = token;
                user.Password = "";
                return Ok(user);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = e.Message, Details = e.InnerException?.Message });
            }
        }

        /// <summary>
        /// Ein "echtes" Logout kann es nicht geben, da wir stateless sind. Dem Benutzer gehört der 
        /// Token, daher können wir ihn nicht "ungültig" machen. 
        /// Manchmal muss man aber etwas bereinigen, daher dieser Mustercode, wie man den 
        /// angemelteten Benutzer identifizieren kann.
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Logout()
        {
            try
            {
                string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                // TODO: Ressourcen bereinigen.
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = e.Message, Details = e.InnerException?.Message });
            }
        }
        [HttpGet]
        public ActionResult<KundenDto> getRoute()
        {
            return Ok("Sie sind in der UserRoute");
        }
    }
}


