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
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace PureFit_REST.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class KundenController : ControllerBase
    {
        private readonly PureFitDbContext _context;

        public KundenController(PureFitDbContext context)
        {
            this._context = context;
        }

        

        [Authorize(Roles = "Kunde")]
        [HttpGet("getData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<UserKundenDto> GetMyKundenData()
        {
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";

            User user = _context.User.Where(d => username.ToLower() == d.U_Name.ToLower()).FirstOrDefault();
            Kunde kunde = _context.Kunde.Find(user.U_Kunde_Nr);
            return Ok(new UserKundenDto
            {
                    Username =  user.U_Name,
                    Password = "Wird aus Sicherheitsgründen nicht angezeigt",
                    Vorname = kunde.K_Vorname,
                    Zuname =  kunde.K_Zuname,
                    Geschlecht =  kunde.K_Geschlecht,
                    Groesse = System.Text.Encoding.UTF8.GetString(kunde.K_Groesse),
                    Gewicht = System.Text.Encoding.UTF8.GetString(kunde.K_Gewicht),
                    GebDatum = System.Text.Encoding.UTF8.GetString(kunde.K_GebDatum),
                    Email = kunde.K_Email != null ? kunde.K_Email : "nicht angegeben",
                    TelefonNr = kunde.K_TelefonNr != null ? kunde.K_TelefonNr : "nicht angegeben",
                Trainingslevel = _context.Trainingslevel.Where(w => w.tr_levelNr == kunde.K_Trainingslevel).Select(s => s.tr_levelname).FirstOrDefault(),
            });
        }
        [Authorize(Roles = "Kunde")]
        [HttpPut("editPersonalData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<User>> EditPersonaldata(KundenDto kunde)
        {
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";

            User user = _context.User.Where(d => username.ToLower() == d.U_Name.ToLower()).FirstOrDefault();
            Kunde kundes = _context.Kunde.Where(w => w.K_Nr == user.U_Kunde_Nr).FirstOrDefault();

            kundes.K_Vorname = kunde.Vorname;
            kundes.K_Zuname = kunde.Zuname;
            kundes.K_Geschlecht = kunde.Geschlecht; 
            kundes.K_Groesse = Encoding.ASCII.GetBytes(kunde.Groesse);
            kundes.K_Gewicht = Encoding.ASCII.GetBytes(kunde.Gewicht);
            kundes.K_GebDatum = Encoding.ASCII.GetBytes(kunde.GebDatum);
            kundes.K_Trainingslevel = _context.Trainingslevel.Where(w => w.tr_levelname == kunde.Trainingslevel).Select(s => s.tr_levelNr).FirstOrDefault();
            _context.SaveChanges();
            return Ok(kunde);

        }

        [Authorize(Roles = "Kunde")]
        [HttpDelete("deleteKundenUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<User>> DeleteKundenUser()
        {
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";

            User user = _context.User.Where(d => username.ToLower() == d.U_Name.ToLower()).FirstOrDefault();
            Kunde kundes = _context.Kunde.Where(w => w.K_Nr == user.U_Kunde_Nr).FirstOrDefault();
            try
            {
                _context.User.Remove(user);
                _context.Kunde.Remove(kundes);
                _context.SaveChanges();

                return Ok(new UserDto());
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        }


    }


   