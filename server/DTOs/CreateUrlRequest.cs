namespace AI_Assisted_URL_Shortener.DTOs
{
    public class CreateUrlRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public string? CustomAlias { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
    }
}
