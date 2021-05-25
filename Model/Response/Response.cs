namespace Quotes.Model.Response
{
    public abstract class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
}