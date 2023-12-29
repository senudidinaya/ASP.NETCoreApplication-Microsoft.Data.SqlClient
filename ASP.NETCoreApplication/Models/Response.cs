namespace ASP.NETCoreApplication.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Message { get; internal set; }
    }
}
