using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PureFit_REST.api.Services;
using PureFit_REST.Dto;
using PureFit_REST.api.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PureFit_REST.api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class AdminController : ControllerBase
    {
        private readonly PureFitDbContext _context;
        private readonly AuthService _authService;
 
        public AdminController(PureFitDbContext context, AuthService authService)
        {
            this._authService = authService;
            this._context = context;
        }

        //regristriert einen neuen Admin -> kann nur von einem Admin gemacht werden
        [Authorize(Roles = "Admin")]
        [HttpPost("register/Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> RegisterAdmin(UserDto user)
        {
            await _authService.CreateUser(user, _context);
            return Ok(user);
        }

        //liefert Daten von ALLEN Kunden
        [Authorize(Roles = "Admin")]
        [HttpGet("getKundenData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<KundenDto> GetALLKunden()
        {
            try
            {
                var result = (from s in _context.Kunde
                             select new KundenDto
                              {
                                  Vorname = s.K_Vorname,
                                  Zuname = s.K_Zuname,
                                  Geschlecht = s.K_Geschlecht,
                                  GebDatum = System.Text.Encoding.UTF8.GetString(s.K_GebDatum),
                                  Email = s.K_Email == null ? s.K_Email : "nicht angegeben",
                                  TelefonNr = s.K_TelefonNr == null ? s.K_Email : "nicht angegeben",
                                 Gewicht = System.Text.Encoding.UTF8.GetString(s.K_Gewicht),
                                  Groesse = System.Text.Encoding.UTF8.GetString(s.K_Groesse),
                                  Trainingslevel = s.K_TrainingslevelNavigation.tr_levelname
                                  
                              });

                return Ok(result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "DB Error" });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = e.Message, Details = e.InnerException?.Message });
            }
        }

        //liefert Daten von ALLEN regristrierten Usern
        [Authorize(Roles = "Admin")]
        [HttpGet("getRegisteredUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<KundenDto> GetgetRegisteredUserAll()
        {
            try
            {
                var result = (from s in _context.User
                              select new UserDto
                              {
                                  Username = s.U_Name
                              });

                return Ok(result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "DB Error" });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = e.Message, Details = e.InnerException?.Message });
            }
        }
    }
}

