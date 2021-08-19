namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class VehicleRegistrationHub : LogMonitoringHub
  {
    public VehicleRegistrationHub() : base("dapr-trafficcontrol") {}
  }
}