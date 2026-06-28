namespace AI_Assisted_URL_Shortener.DTOs
{
    public class UpdateUrlRequest
    {
        public DateTimeOffset? ExpiresAt { get; set; }
        public bool? IsActive { get; set; }
    }
}
