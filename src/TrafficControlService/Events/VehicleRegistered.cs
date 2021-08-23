using System;
using System.Text.Json.Serialization;

namespace TrafficControlService.Events
{
    public class VehicleRegistered
    {
        public VehicleRegistered() {}
        [JsonConstructorAttribute]
        public VehicleRegistered(int lane, string licenseNumber, DateTime timestamp) => (Lane, LicenseNumber, Timestamp) = (lane, licenseNumber, timestamp);
        public int Lane { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime Timestamp { get; set; }
    }
}