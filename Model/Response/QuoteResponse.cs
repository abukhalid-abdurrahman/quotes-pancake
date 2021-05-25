using System;

namespace Quotes.Model.Response
{
    public class QuoteResponse : Response
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        public string QuoteText { get; set; }
        public string CategoryText { get; set; }
        public string AuthorName { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Removed { get; set; }
    }
}