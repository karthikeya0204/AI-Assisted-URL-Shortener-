using System.Text.Json.Serialization;

namespace AI_Assisted_URL_Shortener.Models
{
    public class UrlRecord
    {
        public Guid Id { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public int ClickCount { get; set; }
        public List<string> RedirectAliases { get; set; } = new();
    }
}
