using Microsoft.Extensions.Configuration;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class FineCollectionHub : LogMonitoringHub
  {
    public FineCollectionHub(IConfiguration configuration) : base(configuration) {}
  }
}