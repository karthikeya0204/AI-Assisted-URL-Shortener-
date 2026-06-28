namespace AI_Assisted_URL_Shortener.Models
{
    public class ClickEvent
    {
        public Guid Id { get; set; }
        public Guid UrlId { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
        public string? UserAgent { get; set; }
    }
}
