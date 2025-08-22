using Microsoft.AspNetCore.Mvc;
using TaxiApp.Data.DbContext;
using System.Linq;

namespace TaxiApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxiTypesController : ControllerBase
    {
        private readonly TaxiAppDbContext _context;

        public TaxiTypesController(TaxiAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTaxiTypes()
        {
            var types = _context.TaxiTypes
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.KmPrice,
                    t.BaseFare
                })
                .ToList();

            return Ok(types);
        }
    }
}
