using AI_Assisted_URL_Shortener.DTOs;
using AI_Assisted_URL_Shortener.Models;

namespace AI_Assisted_URL_Shortener.Services
{
    public interface IUrlService
    {
        UrlResponse Create(CreateUrlRequest request);
        IEnumerable<UrlResponse> GetAll();
        UrlResponse? GetByCode(string code);
        UrlResponse Update(string code, UpdateUrlRequest request);
        AnalyticsResponse GetAnalytics(string code);
        UrlRecord? GetForRedirect(string code);
        void LogRedirect(UrlRecord record, string? userAgent);
    }
}
