using k8s;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class KubernetesHub : Hub
  {
    KubernetesClientConfiguration kubernetesClientConfiguration;
    protected IKubernetes iKubernetesClient;
    protected string namespaceName;

    protected IConfiguration configuration;

    public KubernetesHub(IConfiguration configuration)
    {
      this.configuration = configuration;
      kubernetesClientConfiguration = KubernetesClientConfiguration.BuildDefaultConfig();
      iKubernetesClient = new Kubernetes(kubernetesClientConfiguration);
      namespaceName = this.configuration["Kubernetes:DaprNamespace"];
    }
  }
}