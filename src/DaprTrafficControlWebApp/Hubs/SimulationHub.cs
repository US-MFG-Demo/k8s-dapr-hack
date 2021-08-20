using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class SimulationHub : KubernetesHub
  {
    LogMonitoringHub logMonitoringHub;
    public SimulationHub() : base("dapr-trafficcontrol")
    {
      logMonitoringHub = new LogMonitoringHub("dapr-trafficcontrol");
    }

    public async void StartMonitoring(int sinceSeconds, string serviceName)
    {
      await logMonitoringHub.StartMonitoring(sinceSeconds, serviceName);
    }

    public async void StartSimulating()
    {
      await Scale(1);
    }

    public async void StopSimulating()
    {
      await Scale(0);
    }

    private async Task Scale(int replicas)
    {
      var jsonPatch = new JsonPatchDocument<V1Scale>();
      jsonPatch.Replace(e => e.Spec.Replicas, replicas);
      var patch = new V1Patch(jsonPatch, V1Patch.PatchType.JsonPatch);
      await client.PatchNamespacedDeploymentScaleAsync(
        body: patch,
        name: "simulation",
        namespaceParameter: namespaceName
      );
    }
  }
}