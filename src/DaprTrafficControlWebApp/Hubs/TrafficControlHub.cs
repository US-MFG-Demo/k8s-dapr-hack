namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class TrafficControlHub : LogMonitoringHub
  {
    public TrafficControlHub() : base("dapr-trafficcontrol") {}
  }
}