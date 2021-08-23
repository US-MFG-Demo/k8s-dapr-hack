using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class SimulationHub : LogMonitoringHub
  {
    const string SERVICE_NAME = "simulation";
    public SimulationHub() : base("dapr-trafficcontrol")
    {
    }

    public async void StartSimulating()
    {
      IHubCallerClients clients = Clients;
      await Scale(1).ContinueWith(async _ => {
        await clients.All.SendAsync("ReceiveStartSimulating"); });
    }

    public async void StopSimulating()
    {
      IHubCallerClients clients = Clients;
      await Scale(0).ContinueWith(async _ => {
        await clients.All.SendAsync("ReceiveStopSimulating"); });
    }

    private async Task Scale(int replicas)
    {
      var jsonPatch = new JsonPatchDocument<V1Scale>();
      jsonPatch.Replace(e => e.Spec.Replicas, replicas);
      var patch = new V1Patch(jsonPatch, V1Patch.PatchType.JsonPatch);
      await client.PatchNamespacedDeploymentScaleAsync(
        body: patch,
        name: SERVICE_NAME,
        namespaceParameter: namespaceName
      );
    }

    public async void IsSimulating()
    {
      IHubCallerClients clients = Clients;

      var pods = await client.ListNamespacedPodAsync(namespaceName);
      foreach (var pod in pods.Items)
      {
        if (pod.Metadata.Name.Contains(SERVICE_NAME) && pod.Status.Phase == "Running")
        {
          await clients.All.SendAsync("ReceiveIsSimulating", true);
          return;
        }
      }

      await clients.All.SendAsync("ReceiveIsSimulating", false);
    }
  }
}