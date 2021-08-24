using Microsoft.Extensions.Configuration;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class TrafficControlHub : LogMonitoringHub
  {
    public TrafficControlHub(IConfiguration configuration) : base(configuration) {}
  }
}