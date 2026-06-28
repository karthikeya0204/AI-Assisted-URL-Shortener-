namespace AI_Assisted_URL_Shortener.DTOs
{
    public class UrlResponse
    {
        public Guid Id { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public int ClickCount { get; set; }
    }
}
