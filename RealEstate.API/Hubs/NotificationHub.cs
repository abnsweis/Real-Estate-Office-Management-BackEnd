using Microsoft.AspNetCore.SignalR;
using RealEstate.API.Hubs.Interfaces;

namespace RealEstate.API.Hubs
{
    public class NotificationHub : Hub<INotificationHub>
    {
        public override async Task OnConnectedAsync()
        {

            await Clients.All.Received(Context.ConnectionId);
             
        }
         
        public async Task Send(string message)
        {
            await Clients.All.Received(message);
        }
    }
}
