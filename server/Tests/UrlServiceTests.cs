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
        public UrlRecord? GetByShortCode(string shortCode) => _records.FirstOrDefault(x => x.ShortCode == shortCode);
        public IEnumerable<ClickEvent> GetClickEvents(string shortCode) => _events.Where(x => x.ShortCode == shortCode);
        public bool ExistsByShortCode(string shortCode) => _records.Any(x => x.ShortCode == shortCode);
        public void Update(UrlRecord record)
        {
            var existing = _records.FirstOrDefault(x => x.Id == record.Id);
            if (existing is null) return;
            existing.IsActive = record.IsActive;
            existing.ExpiresAt = record.ExpiresAt;
            existing.ClickCount = record.ClickCount;
            existing.OriginalUrl = record.OriginalUrl;
        }
    }
}
