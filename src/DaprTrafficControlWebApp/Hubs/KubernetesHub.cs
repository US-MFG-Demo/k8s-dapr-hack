using k8s;
using Microsoft.AspNetCore.SignalR;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class KubernetesHub : Hub
  {
    KubernetesClientConfiguration config;
    protected IKubernetes client;
    protected string namespaceName;

    public KubernetesHub(string namespaceName)
    {
      config = KubernetesClientConfiguration.BuildDefaultConfig();
      client = new Kubernetes(config);
      this.namespaceName = namespaceName;
    }
  }
}