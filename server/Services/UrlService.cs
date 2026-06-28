using AI_Assisted_URL_Shortener.DTOs;
using AI_Assisted_URL_Shortener.Models;
using AI_Assisted_URL_Shortener.Repositories;
using System.Text.RegularExpressions;

namespace AI_Assisted_URL_Shortener.Services
{
    public class UrlService : IUrlService
    {
        private readonly IUrlRepository _repository;
        private readonly Regex _urlRegex = new(@"^https?://.+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public UrlService(IUrlRepository repository)
        {
            _repository = repository;
        }

        public UrlResponse Create(CreateUrlRequest request)
        {
            ValidateRequest(request);

            var shortCode = GenerateShortCode(request.CustomAlias);
            var record = new UrlRecord
            {
                Id = Guid.NewGuid(),
                ShortCode = shortCode,
                OriginalUrl = request.OriginalUrl.Trim(),
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = request.ExpiresAt,
                IsActive = true,
                ClickCount = 0
            };

            _repository.Add(record);
            return Map(record);
        }

        public IEnumerable<UrlResponse> GetAll()
        {
            return _repository.GetAll().Select(Map);
        }

        public UrlResponse? GetByCode(string code)
        {
            var record = _repository.GetByShortCode(code);
            return record is null ? null : Map(record);
        }

        public UrlResponse Update(string code, UpdateUrlRequest request)
        {
            var record = _repository.GetByShortCode(code) ?? throw new InvalidOperationException("URL not found.");
            var previousShortCode = code;

            if (!string.IsNullOrWhiteSpace(request.CustomAlias))
            {
                var alias = request.CustomAlias.Trim();
                if (alias != record.ShortCode && _repository.ExistsByShortCode(alias))
                {
                    throw new ArgumentException("Custom alias is already in use.");
                }

                if (!string.IsNullOrWhiteSpace(previousShortCode)
                    && !string.Equals(previousShortCode, alias, StringComparison.OrdinalIgnoreCase)
                    && !record.RedirectAliases.Any(existing => string.Equals(existing, previousShortCode, StringComparison.OrdinalIgnoreCase)))
                {
                    record.RedirectAliases.Add(previousShortCode);
                }

                record.ShortCode = alias;
            }

            if (request.ExpiresAt.HasValue)
            {
                record.ExpiresAt = request.ExpiresAt.Value;
            }

            if (request.IsActive.HasValue)
            {
                record.IsActive = request.IsActive.Value;
            }

            _repository.Update(record);
            return Map(record);
        }

        public AnalyticsResponse GetAnalytics(string code)
        {
            var record = _repository.GetByShortCode(code) ?? throw new InvalidOperationException("URL not found.");
            var events = _repository.GetClickEvents(code);
            return new AnalyticsResponse
            {
                ShortCode = record.ShortCode,
                TotalClicks = events.Count(),
                ExpiresAt = record.ExpiresAt,
                IsActive = record.IsActive
            };
        }

        public UrlRecord? GetForRedirect(string code)
        {
            var record = _repository.GetByShortCode(code);
            if (record is null || !record.IsActive)
            {
                return null;
            }
            if (record.ExpiresAt.HasValue && record.ExpiresAt.Value < DateTimeOffset.UtcNow)
            {
                return null;
            }
            return record;
        }

        public void LogRedirect(UrlRecord record, string? userAgent)
        {
            record.ClickCount++;
            _repository.Update(record);
            _repository.AddClickEvent(new ClickEvent
            {
                Id = Guid.NewGuid(),
                UrlId = record.Id,
                ShortCode = record.ShortCode,
                Timestamp = DateTimeOffset.UtcNow,
                UserAgent = userAgent
            });
        }

        private void ValidateRequest(CreateUrlRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OriginalUrl))
            {
                throw new ArgumentException("OriginalUrl is required.");
            }
            var trimmed = request.OriginalUrl.Trim();
            if (!_urlRegex.IsMatch(trimmed))
            {
                throw new ArgumentException("OriginalUrl must start with http:// or https://.");
            }
            if (!string.IsNullOrWhiteSpace(request.CustomAlias) && _repository.ExistsByShortCode(request.CustomAlias.Trim()))
            {
                throw new ArgumentException("Custom alias is already in use.");
            }
        }

        private string GenerateShortCode(string? requestedAlias)
        {
            if (!string.IsNullOrWhiteSpace(requestedAlias))
            {
                return requestedAlias.Trim();
            }

            string code;
            do
            {
                code = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Replace("=", string.Empty)
                    .Replace("+", string.Empty)
                    .Replace("/", string.Empty)
                    .Substring(0, 8);
            }
            while (_repository.ExistsByShortCode(code));

            return code;
        }

        private static UrlResponse Map(UrlRecord record)
        {
            var encodedShortCode = Uri.EscapeDataString(record.ShortCode);

            return new UrlResponse
            {
                Id = record.Id,
                ShortCode = record.ShortCode,
                ShortUrl = $"http://localhost:5000/{encodedShortCode}",
                OriginalUrl = record.OriginalUrl,
                CreatedAt = record.CreatedAt,
                ExpiresAt = record.ExpiresAt,
                IsActive = record.IsActive,
                ClickCount = record.ClickCount
            };
        }
    }
}
