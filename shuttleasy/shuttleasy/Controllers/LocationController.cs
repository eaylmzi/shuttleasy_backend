using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using shuttleasy.LocationSocket;

namespace shuttleasy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : Controller
    {
        private readonly IHubContext<LocationHub> _hubContext;

        public LocationController(IHubContext<LocationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post(double latitude, double longitude)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveLocation", latitude, longitude);
            return Ok();
        }
    }
}
