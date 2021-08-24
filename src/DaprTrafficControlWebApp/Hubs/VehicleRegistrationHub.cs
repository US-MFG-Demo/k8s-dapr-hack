using Microsoft.Extensions.Configuration;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class VehicleRegistrationHub : LogMonitoringHub
  {
    public VehicleRegistrationHub(IConfiguration configuration) : base(configuration) {}
  }
}