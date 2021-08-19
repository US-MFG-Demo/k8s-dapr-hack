using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DaprTrafficControlWebApp.Data;
using k8s;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Rest;

namespace DaprTrafficControlWebApp.Server.Hubs
{
  public class TrafficControlHub : Hub
  {
    KubernetesClientConfiguration config;
    IKubernetes client;

    public TrafficControlHub()
    {
      config = KubernetesClientConfiguration.BuildDefaultConfig();
      client = new Kubernetes(config);
    }
    public async void StartMonitoring(int sinceSeconds, string serviceName)
    {
      var pods = client.ListNamespacedPod("dapr-trafficcontrol");

      foreach (var pod in pods.Items)
      {
        if (pod.Metadata.Name.Contains(serviceName))
        {
          IHubCallerClients clients = Clients;

          var response = await client.ReadNamespacedPodLogWithHttpMessagesAsync(
            name: pod.Metadata.Name,
            namespaceParameter: pod.Metadata.NamespaceProperty,
            container: serviceName,
            sinceSeconds: sinceSeconds,
            timestamps: true);

          await SendMessage(response, clients);
        }
      }

      //await Clients.All.SendAsync("ReceiveMessage", "asdf");
    }

    private async Task SendMessage(HttpOperationResponse<Stream> response, IHubCallerClients clients)
    {
      var stream = response.Body;
      StreamReader reader = new StreamReader(stream);
      string result = await reader.ReadToEndAsync();

      List<LogMessage> messages = new List<LogMessage>();

      var tempMessages = result.Split('\n');
      foreach(var message in tempMessages)
      {
        if(!message.Contains('\u001b')) {
          var tempMessage = message.Split(' ', 2, StringSplitOptions.TrimEntries);
          if(tempMessage.Length == 2)
          {
            messages.Add(new LogMessage(tempMessage[0], tempMessage[1]));
          }
        }
      }

      await clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(messages));
    }
  }
}