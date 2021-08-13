using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class TrafficControlHub : Hub
  {
    public async Task SendLogMessage()
    {
      await Clients.All.SendAsync("ReceiveMessage", "Hello world!");
    }   
  }   
}