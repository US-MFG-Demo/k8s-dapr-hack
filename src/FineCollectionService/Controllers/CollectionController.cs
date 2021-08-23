using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FineCollectionService.DomainServices;
using FineCollectionService.Helpers;
using FineCollectionService.Models;
using FineCollectionService.Proxies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dapr.Client;
using Newtonsoft.Json.Linq;

namespace FineCollectionService.Controllers
{
    [ApiController]
    [Route("")]
    public class CollectionController : ControllerBase
    {
        private static string _fineCalculatorLicenseKey;
        private readonly ILogger<CollectionController> _logger;
        private readonly IFineCalculator _fineCalculator;
        private readonly VehicleRegistrationService _vehicleRegistrationService;

        public CollectionController(ILogger<CollectionController> logger,
            IFineCalculator fineCalculator, VehicleRegistrationService vehicleRegistrationService, [FromServices] DaprClient daprClient, HttpClient httpClient)
        {
            _logger = logger;
            _fineCalculator = fineCalculator;
            _vehicleRegistrationService = vehicleRegistrationService;

            // set finecalculator component license-key
            if (_fineCalculatorLicenseKey == null)
            {
                //_fineCalculatorLicenseKey = "HX783-K2L7V-CRJ4A-5PN1G";
                var secrets = daprClient.GetSecretAsync(
                    "trafficcontrol-secrets", "finecalculator-licensekey").Result;
                _logger.LogInformation($"LICENSE KEY: {secrets["finecalculator-licensekey"]}");
                _fineCalculatorLicenseKey = secrets["finecalculator-licensekey"];
            }

            httpClient.DefaultRequestHeaders.Clear();
            var response = httpClient.GetAsync("http://localhost:3500/v1.0/secrets/trafficcontrol-secrets/finecalculator-licensekey").Result;
            _logger.LogInformation(response.Content.ReadAsStringAsync().Result);
        }

        [Route("collectfine")]
        [HttpPost()]
        //public async Task<ActionResult> CollectFine(SpeedingViolation speedingViolation)
        public async Task<ActionResult> CollectFine([FromBody] System.Text.Json.JsonDocument cloudevent, [FromServices] DaprClient daprClient, [FromServices] HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Clear();
            var response = httpClient.GetAsync("http://localhost:3500/v1.0/secrets/trafficcontrol-secrets/finecalculator-licensekey").Result;
            _logger.LogInformation(response.Content.ReadAsStringAsync().Result);

            _logger.LogInformation($"LICENSE KEY: {_fineCalculatorLicenseKey}");
            var data = cloudevent.RootElement.GetProperty("data");
            var speedingViolation = new SpeedingViolation
            {
                VehicleId = data.GetProperty("vehicleId").GetString(),
                RoadId = data.GetProperty("roadId").GetString(),
                Timestamp = data.GetProperty("timestamp").GetDateTime(),
                ViolationInKmh = data.GetProperty("violationInKmh").GetInt32()
            };

            decimal fine = _fineCalculator.CalculateFine(_fineCalculatorLicenseKey, speedingViolation.ViolationInKmh);

            // get owner info
            var vehicleInfo = await _vehicleRegistrationService.GetVehicleInfo(speedingViolation.VehicleId);

            // log fine
            string fineString = fine == 0 ? "tbd by the prosecutor" : $"{fine} Euro";
            _logger.LogInformation($"Sent speeding ticket to {vehicleInfo.OwnerName}. " +
                $"Road: {speedingViolation.RoadId}, Licensenumber: {speedingViolation.VehicleId}, " +
                $"Vehicle: {vehicleInfo.Brand} {vehicleInfo.Model}, " +
                $"Violation: {speedingViolation.ViolationInKmh} Km/h, Fine: {fineString}, " +
                $"On: {speedingViolation.Timestamp.ToString("dd-MM-yyyy")} " +
                $"at {speedingViolation.Timestamp.ToString("hh:mm:ss")}.");

            // send fine by email
            var body = EmailUtils.CreateEmailBody(speedingViolation, vehicleInfo, fineString);
            var metadata = new Dictionary<string, string>
            {
                ["emailFrom"] = "noreply@cfca.gov",
                ["emailTo"] = "jordanbean@microsoft.com",//vehicleInfo.OwnerEmail,
                ["subject"] = $"Speeding violation on the {speedingViolation.RoadId}"
            };

            dynamic email = new JObject();
            email.from = "noreply@cfca.gov";
            email.to = "jordanbean@microsoft.com";
            email.subject = $"Speeding violation on the {speedingViolation.RoadId}";
            email.body = body;

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(email);

            await daprClient.InvokeBindingAsync("sendmail", "create", jsonString);

            return Ok();
        }
    }
}
