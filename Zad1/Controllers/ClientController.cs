using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zad1.Services;

namespace Zad1.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientController : ControllerBase
    {

        private readonly IDbService _dbService;

        public ClientController(IDbService dbService)
        {
            this._dbService = dbService;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> removeClient(int id)
        {
            try
            {
                await _dbService.removeClient(id);
                return Ok("Usunięto klienta");
            }catch(Exception e)
            {
                if (e.Message.Equals("Nie znaleziono klienta"))
                {
                    return NotFound(e.Message);
                }
                if (e.Message.Equals("Nie można usunąć klienta"))
                {
                    return BadRequest("Nie można usunąć klienta ponieważ klient jest przypisany do wycieczki");
                }
                return StatusCode(500);
            }
        }
    }
}
