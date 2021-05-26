using System;

namespace Quotes.Model.DTOs.Response
{
    public class QuoteStatisticsResponse : QuoteResponse
    {
        public string Action { get; set; }
        public string Period { get; set; }
        public DateTime ActionDate { get; set; }
    }
}