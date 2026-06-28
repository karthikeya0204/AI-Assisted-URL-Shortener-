namespace AI_Assisted_URL_Shortener.DTOs
{
    public class AnalyticsResponse
    {
        public string ShortCode { get; set; } = string.Empty;
        public int TotalClicks { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
