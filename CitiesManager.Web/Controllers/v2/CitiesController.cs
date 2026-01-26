using Asp.Versioning;
using CitiesManager.Web.DataBaseContext;
using CitiesManager.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitiesManager.Web.Controllers.v2
{
    [ApiVersion("2.0")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        /// <summary>
        /// To get all cities name from database
        /// </summary>
        /// <returns>List of cities name</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string?>>> GetCities()
        {
            return await _context.Cities
                .OrderBy(c => c.CityName)
                .Select(c => c.CityName)
                .ToListAsync();
        }

        // GET: api/Cities/5
        [HttpGet("{CityId}")]
        public async Task<ActionResult<string?>> GetCity(Guid CityId)
        {
            var city = await _context.Cities.FindAsync(CityId);

            if (city == null)
            {
                return NotFound();
            }

            return city.CityName;
        }
    }
}
