using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Simulation.Events;

namespace Simulation.Proxies
{
    public class HttpTrafficControlService : ITrafficControlService
    {
        const string IOT_HUB_SAS_TOKEN = "SharedAccessSignature sr=iothub-dapr-ussc-demo.azure-devices.net%2Fdevices%2Fsimulation&sig=SrjNgGxIXrClLqRvIy40rs0eSToHNXOFINe3ElGQY3k%3D&se=1625767171";
        private HttpClient _httpClient;

        public HttpTrafficControlService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SendVehicleEntry(VehicleRegistered vehicleRegistered)
        {
            var eventJson = JsonSerializer.Serialize(vehicleRegistered);
            var message = JsonContent.Create<VehicleRegistered>(vehicleRegistered);
            //_httpClient.PostAsync("http://trafficcontrolservice.e13e6fb6d2534a41ae60.southcentralus.aksapp.io/v1.0/invoke/trafficcontrolservice/method/entrycam", message).Wait();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("authorization", IOT_HUB_SAS_TOKEN);
            _httpClient.PostAsync("https://iothub-dapr-ussc-demo.azure-devices.net/devices/simulation/messages/events?api-version=2018-06-30", message).Wait();
        }

        public void SendVehicleExit(VehicleRegistered vehicleRegistered)
        {
            var eventJson = JsonSerializer.Serialize(vehicleRegistered);
            var message = JsonContent.Create<VehicleRegistered>(vehicleRegistered);
            //_httpClient.PostAsync("http://trafficcontrolservice.e13e6fb6d2534a41ae60.southcentralus.aksapp.io/v1.0/invoke/trafficcontrolservice/method/exitcam", message).Wait();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("authorization", IOT_HUB_SAS_TOKEN);
            _httpClient.PostAsync("https://iothub-dapr-ussc-demo.azure-devices.net/devices/simulation/messages/events?api-version=2018-06-30", message).Wait();
        }
    }
}