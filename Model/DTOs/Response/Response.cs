namespace Quotes.Model.DTOs.Response
{
    public abstract class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
}