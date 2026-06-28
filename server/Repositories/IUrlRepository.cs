using AI_Assisted_URL_Shortener.Models;

namespace AI_Assisted_URL_Shortener.Repositories
{
    public interface IUrlRepository
    {
        IEnumerable<UrlRecord> GetAll();
        UrlRecord? GetByShortCode(string shortCode);
        void Add(UrlRecord record);
        void Update(UrlRecord record);
        void AddClickEvent(ClickEvent clickEvent);
        IEnumerable<ClickEvent> GetClickEvents(string shortCode);
        bool ExistsByShortCode(string shortCode);
    }
}
