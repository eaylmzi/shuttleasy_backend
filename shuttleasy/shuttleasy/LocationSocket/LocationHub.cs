using Microsoft.AspNetCore.SignalR;

namespace shuttleasy.LocationSocket
{
    public class LocationHub : Hub
    {
        public async Task SendLocation(double latitude, double longitude)
        {
            await Clients.Others.SendAsync("ReceiveLocation", latitude, longitude);
        }
    }
}
