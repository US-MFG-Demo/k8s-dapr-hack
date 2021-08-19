namespace DaprTrafficControlWebApp.Data {
  public class LogMessage
  {
    public string Timestamp { get; set; }
    public string Message { get; set; }

    public LogMessage(string timestamp, string message)
    {
      Timestamp = timestamp;
      Message = message;
    }
  }
}