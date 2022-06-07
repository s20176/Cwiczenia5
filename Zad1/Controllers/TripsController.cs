using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zad1.Models.DTO;
using Zad1.Services;

namespace Zad1.Controllers
{
    [ApiController]
    [Route("api/trips")]
    public class TripsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public TripsController(IDbService dbService)
        {
            this._dbService = dbService;
        }

        [HttpGet]
         public async Task<IActionResult> getTrips()
        {
            var trips= await _dbService.getTrips();
            return Ok(trips);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> deleteTrip(int id)
        {
            await _dbService.removeTrip(id);
            return Ok("Removed trip");
        }

        [HttpPost]
        [Route("{id}/clients")]
        public async Task<IActionResult> addClientToTrip(int id,ClientTripDTO clientTrip)
        {
            try
            {
                await _dbService.addClientToTrip(id, clientTrip);
                return Ok("Dodano klienta do wycieczki");
            }catch(Exception e)
            {
                if (e.Message.Equals("Klient jest już zapisany na tą wycieczkę"))
                {
                    return BadRequest(e.Message);
                }
                if (e.Message.Equals("Wycieczka nie istnieje"))
                {
                    return NotFound(e.Message);
                }
                return StatusCode(500);
            }
        }
    }
}
