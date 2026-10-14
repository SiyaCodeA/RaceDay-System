
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;


namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseCheckController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public DatabaseCheckController(RaceDayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            bool connected = await _context.Database.CanConnectAsync();

            return Ok(new
            {
                database = "RaceDayDB",
                connected = connected
            });
        }
    }
}
