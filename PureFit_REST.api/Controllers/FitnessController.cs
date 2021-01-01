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
using System.Security.Claims;
using System.Text;

namespace PureFit_REST.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class FitnessController : ControllerBase
    {
        private readonly PureFitDbContext _context;
        /// <summary>
        /// Konstruktor. Setzt den DB Context.
        /// </summary>
        /// <param name="context">Der über services.AddDbContext() gesetzte Context.</param>
        public FitnessController(PureFitDbContext context)
        {
            this._context = context;
        }

        //Get ALL FitnessÜbungen
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<FItnessuebungDto>> Get()
        {
            try
            {
                var allUebungen = from c in _context.Fitness_Uebungen
                                  select new FItnessuebungDto
                                  {
                                      UebungsNr = c.FU_Nr,
                                      Beschreibung = c.FU_Beschreibung,
                                      Dauer = System.Text.Encoding.UTF8.GetString(c.FU_Dauer),
                                      Image = c.FU_ImageSorce,
                                      UebungsName = c.FU_Name,
                                      Video = c.FU_VideoSorce,
                                      Kalorien = System.Text.Encoding.UTF8.GetString(c.FU_Kalorien),
                                      MuskelName = c.FU_Muskel_NrNavigation.M_NameMuskel,
                                      SchwierigkeitsgradName = c.FU_SchwierigkeitsgradNavigation.S_Name,
                                      Wiederholungen =  c.FU_Wiederholungen
                                      
                                      
                                  };
                return Ok(allUebungen);
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

        /// <summary>
        /// GET /api/classes/(klassenname): 
        /// Liefert Details zu einer Klasse als JSON Object.
        /// </summary>
        /// <param name="id">Eindeutiger Name, nach dem in der Datenbank gesucht wird.</param>
        /// <returns>
        /// HTTP 200: JSON Object mit den Klassendetails oder leer be nicht gefundener Klasse.
        /// HTTP 401: Nicht authentifiziert.
        /// HTTP 403: Nicht autorisiert, der User hat nicht die Rolle Teacher.
        /// HTTP 500: Datenbank- oder Serverfehler.
        /// </returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<FItnessuebungDto> GetUebungId(long id)
        {
            try
            {
                // Mit Find() bekommen wir keine Navigation Properties. Daher filtern wir mit Where.
                // Durch FirstOrDefault wird ein JSON Object und kein Array geliefert.
                var result = (from c in _context.Fitness_Uebungen
                              where c.FU_Nr == id
                              select new FItnessuebungDto
                              {
                                  UebungsNr = c.FU_Nr,
                                  Beschreibung = c.FU_Beschreibung,
                                  Dauer = System.Text.Encoding.UTF8.GetString(c.FU_Dauer),
                                  Image = c.FU_ImageSorce,
                                  UebungsName = c.FU_Name,
                                  Video = c.FU_VideoSorce,
                                  Kalorien = System.Text.Encoding.UTF8.GetString(c.FU_Kalorien),
                                  MuskelName = c.FU_Muskel_NrNavigation.M_körperteilName,
                                  SchwierigkeitsgradName = c.FU_SchwierigkeitsgradNavigation.S_Name,
                                  Wiederholungen = c.FU_Wiederholungen
                              }).FirstOrDefault();
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




        //Get ALL Trainingslevel
        [AllowAnonymous]
        [HttpGet("trainingslevel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<FItnessuebungDto>> GetTrainingslevel()
        {
            try
            {
                var allLevel = from c in _context.Trainingslevel
                                  select new TrainingslevelDto
                                  {
                                    Name =c.tr_levelname,
                                    Beschreibung= c.tr_beschreibung
                                      
                                  };
                return Ok(allLevel);
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

        [Authorize(Roles = "Kunde")]
        [HttpGet("getFitnessVerlauf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<IEnumerable<FitnessHistoryDto>> getFitnessVerlauf()
        {
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";

            User user = _context.User.Where(d => username.ToLower() == d.U_Name.ToLower()).FirstOrDefault();
            Kunde kunde = _context.Kunde.Find(user.U_Kunde_Nr);
            var allUebung = from c in _context.Fitness_history
                            where kunde.K_Nr == c.FH_Kunde_Nr
                           select new FitnessHistoryDto
                           {
                               Date = System.Text.Encoding.UTF8.GetString(c.FH_Date),
                               Bewertung = System.Text.Encoding.UTF8.GetString(c.FH_Bewertung),
                               SchwierigkeitsgradName = c.FH_Fitness_Uebungen_NrNavigation.FU_SchwierigkeitsgradNavigation.S_Name,
                               Beschreibung = c.FH_Fitness_Uebungen_NrNavigation.FU_Beschreibung,
                               Dauer = System.Text.Encoding.UTF8.GetString(c.FH_Fitness_Uebungen_NrNavigation.FU_Dauer),
                               Kalorien = System.Text.Encoding.UTF8.GetString(c.FH_Fitness_Uebungen_NrNavigation.FU_Kalorien),
                               KundenNr = c.FH_Kunde_Nr,
                               MuskelName = c.FH_Fitness_Uebungen_NrNavigation.FU_Muskel_NrNavigation.M_körperteilName,
                               UebungsNr = c.FH_Fitness_Uebungen_Nr,
                               UebungsName = c.FH_Fitness_Uebungen_NrNavigation.FU_Name,
                               Wiederholungen = c.FH_Fitness_Uebungen_NrNavigation.FU_Wiederholungen,  
                               Image = c.FH_Fitness_Uebungen_NrNavigation.FU_ImageSorce
                           };
            return Ok(allUebung);
        }
        [Authorize(Roles = "Kunde")]
        [HttpPost("addWorkout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<FitnessHistoryDto>> addWorkout([FromBody]FitnessHistoryDto fitnessHistoryDto)
        {
            try
            {
                string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
                User user = _context.User.Where(d => username.ToLower() == d.U_Name.ToLower()).FirstOrDefault();
                Kunde kunde = _context.Kunde.Find(user.U_Kunde_Nr);

                Fitness_history history = new Fitness_history
                {
                    FH_Date = Encoding.ASCII.GetBytes(fitnessHistoryDto.Date),
                    FH_Bewertung = Encoding.ASCII.GetBytes(fitnessHistoryDto.Bewertung),
                   FH_Kunde_Nr = long.Parse("" + kunde.K_Nr),
                   FH_Fitness_Uebungen_Nr = fitnessHistoryDto.UebungsNr
                };
                _context.Fitness_history.Add(history);
                _context.SaveChanges();

                return Ok(fitnessHistoryDto);
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
