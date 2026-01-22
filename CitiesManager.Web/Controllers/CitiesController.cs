using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public string MyCity()
        {
            return "I love my city.";
        }
    }
}
