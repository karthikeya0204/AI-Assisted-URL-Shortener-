using AI_Assisted_URL_Shortener.DTOs;
using AI_Assisted_URL_Shortener.Models;
using AI_Assisted_URL_Shortener.Repositories;
using AI_Assisted_URL_Shortener.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AI_Assisted_URL_Shortener.Tests
{
    public class UrlServiceTests
    {
        [Fact]
        public void Create_ValidRequest_ReturnsShortUrl()
        {
            var repository = new InMemoryUrlRepository();
            var service = new UrlService(repository);

            var response = service.Create(new CreateUrlRequest { OriginalUrl = "https://example.com" });

            Assert.Equal("https://example.com", response.OriginalUrl);
            Assert.False(string.IsNullOrWhiteSpace(response.ShortCode));
        }

        [Fact]
        public void Create_InvalidUrl_ThrowsArgumentException()
        {
            var repository = new InMemoryUrlRepository();
            var service = new UrlService(repository);

            Assert.Throws<ArgumentException>(() => service.Create(new CreateUrlRequest { OriginalUrl = "ftp://bad.example" }));
        }

        [Fact]
        public void Create_CustomAlias_ReturnsCustomShortCode()
        {
            var repository = new InMemoryUrlRepository();
            var service = new UrlService(repository);

            var response = service.Create(new CreateUrlRequest { OriginalUrl = "https://example.com", CustomAlias = "myalias" });

            Assert.Equal("myalias", response.ShortCode);
        }

        [Fact]
        public void Create_CustomAliasWithSpaces_EncodesShortUrl()
        {
            var repository = new InMemoryUrlRepository();
            var service = new UrlService(repository);

            var response = service.Create(new CreateUrlRequest { OriginalUrl = "https://example.com", CustomAlias = "my alias" });

            Assert.Equal("my alias", response.ShortCode);
            Assert.Equal("http://localhost:5000/my%20alias", response.ShortUrl);
        }

        [Fact]
        public void Update_CustomAlias_ChangesShortCodeAndKeepsPreviousAlias()
        {
            var repository = new InMemoryUrlRepository();
            var service = new UrlService(repository);

            service.Create(new CreateUrlRequest { OriginalUrl = "https://example.com", CustomAlias = "oldalias" });

            var updated = service.Update("oldalias", new UpdateUrlRequest { CustomAlias = "newalias" });

            Assert.Equal("newalias", updated.ShortCode);
            Assert.NotNull(service.GetByCode("newalias"));
            Assert.NotNull(service.GetByCode("oldalias"));
        }

        [Fact]
        public void Update_CustomAliasToExistingAlias_ThrowsArgumentException()
        {
            var repository = new InMemoryUrlRepository();
            var service = new UrlService(repository);

            service.Create(new CreateUrlRequest { OriginalUrl = "https://example.com", CustomAlias = "oldalias" });
            service.Create(new CreateUrlRequest { OriginalUrl = "https://example.org", CustomAlias = "takenalias" });

            Assert.Throws<ArgumentException>(() => service.Update("oldalias", new UpdateUrlRequest { CustomAlias = "takenalias" }));
        }

        [Fact]
        public void Update_CustomAlias_KeepsOldAliasWorkingForRedirect()
        {
            var repository = new InMemoryUrlRepository();
            var service = new UrlService(repository);

            service.Create(new CreateUrlRequest { OriginalUrl = "https://example.com", CustomAlias = "oldalias" });

            service.Update("oldalias", new UpdateUrlRequest { CustomAlias = "newalias" });

            var record = service.GetForRedirect("oldalias");

            Assert.NotNull(record);
            Assert.Equal("https://example.com", record.OriginalUrl);
            Assert.Equal("newalias", record.ShortCode);
        }

        [Fact]
        public void Update_ThroughPreviousAlias_PreservesRedirectFallback()
        {
            var repository = new InMemoryUrlRepository();
            var service = new UrlService(repository);

            service.Create(new CreateUrlRequest { OriginalUrl = "https://example.com", CustomAlias = "oldalias" });
            service.Update("oldalias", new UpdateUrlRequest { CustomAlias = "newalias" });
            service.Update("oldalias", new UpdateUrlRequest { CustomAlias = "latestalias" });

            var record = service.GetForRedirect("oldalias");

            Assert.NotNull(record);
            Assert.Equal("latestalias", record.ShortCode);
        }

        [Fact]
        public void GetForRedirect_ExpiredUrl_ReturnsNull()
        {
            var repository = new InMemoryUrlRepository();
            repository.Add(new UrlRecord
            {
                Id = Guid.NewGuid(),
                ShortCode = "expired",
                OriginalUrl = "https://example.com",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-2),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1),
                IsActive = true,
                ClickCount = 0
            });
            var service = new UrlService(repository);

            var result = service.GetForRedirect("expired");

            Assert.Null(result);
        }
    }

    public class InMemoryUrlRepository : IUrlRepository
    {
        private readonly List<UrlRecord> _records = new();
        private readonly List<ClickEvent> _events = new();

        public void Add(UrlRecord record) => _records.Add(record);
        public void AddClickEvent(ClickEvent clickEvent) => _events.Add(clickEvent);
        public IEnumerable<UrlRecord> GetAll() => _records;
        public UrlRecord? GetByShortCode(string shortCode) => _records.FirstOrDefault(x =>
            string.Equals(x.ShortCode, shortCode, StringComparison.OrdinalIgnoreCase) ||
            x.RedirectAliases.Any(alias => string.Equals(alias, shortCode, StringComparison.OrdinalIgnoreCase)));
        public IEnumerable<ClickEvent> GetClickEvents(string shortCode) => _events.Where(x => x.ShortCode == shortCode);
        public bool ExistsByShortCode(string shortCode) => _records.Any(x =>
            string.Equals(x.ShortCode, shortCode, StringComparison.OrdinalIgnoreCase) ||
            x.RedirectAliases.Any(alias => string.Equals(alias, shortCode, StringComparison.OrdinalIgnoreCase)));
        public void Update(UrlRecord record)
        {
            var existing = _records.FirstOrDefault(x => x.Id == record.Id);
            if (existing is null) return;
            existing.IsActive = record.IsActive;
            existing.ExpiresAt = record.ExpiresAt;
            existing.ClickCount = record.ClickCount;
            existing.OriginalUrl = record.OriginalUrl;
            existing.ShortCode = record.ShortCode;
            existing.RedirectAliases = record.RedirectAliases.ToList();
        }
    }
}
