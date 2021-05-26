namespace Quotes.Model.DTOs.Response
{
    public class QuoteResponse
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        public string QuoteText { get; set; }
        public string CategoryText { get; set; }
        public string AuthorName { get; set; }
    }
}