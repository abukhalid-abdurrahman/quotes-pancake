namespace Quotes.Model.DTOs.Response
{
    public class AuthorResponse : Response
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Token { get; set; }
    }
}