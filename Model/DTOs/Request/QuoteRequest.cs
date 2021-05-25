namespace Quotes.Model.DTOs.Request
{
    public class QuoteRequest
    {
        public int? QuoteId { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public string QuoteText { get; set; }
    }
}