namespace AI_Assisted_URL_Shortener.DTOs
{
    public class UpdateUrlRequest
    {
        public string? CustomAlias { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public bool? IsActive { get; set; }
    }
}
