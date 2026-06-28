using AI_Assisted_URL_Shortener.Models;
using System.Text.Json;

namespace AI_Assisted_URL_Shortener.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly string _dataFolder;
        private readonly string _urlsPath;
        private readonly string _eventsPath;
        private readonly object _lock = new();

        public UrlRepository()
        {
            var projectRoot = FindProjectRoot();
            _dataFolder = Path.GetFullPath(Path.Combine(projectRoot, "data"));
            Directory.CreateDirectory(_dataFolder);
            _urlsPath = Path.Combine(_dataFolder, "urls.json");
            _eventsPath = Path.Combine(_dataFolder, "click-events.json");
            MigrateExistingDataIfNeeded();
            EnsureFile(_urlsPath);
            EnsureFile(_eventsPath);
        }

        public IEnumerable<UrlRecord> GetAll()
        {
            return ReadRecords(_urlsPath);
        }

        public UrlRecord? GetByShortCode(string shortCode)
        {
            return GetAll().FirstOrDefault(x =>
                string.Equals(x.ShortCode, shortCode, StringComparison.OrdinalIgnoreCase) ||
                x.RedirectAliases.Any(alias => string.Equals(alias, shortCode, StringComparison.OrdinalIgnoreCase)));
        }

        public void Add(UrlRecord record)
        {
            lock (_lock)
            {
                var records = ReadRecords(_urlsPath).ToList();
                records.Add(record);
                WriteRecords(_urlsPath, records);
            }
        }

        public void Update(UrlRecord record)
        {
            lock (_lock)
            {
                var records = ReadRecords(_urlsPath).ToList();
                var index = records.FindIndex(x => x.Id == record.Id);
                if (index >= 0)
                {
                    records[index] = record;
                    WriteRecords(_urlsPath, records);
                }
            }
        }

        public void AddClickEvent(ClickEvent clickEvent)
        {
            lock (_lock)
            {
                var events = ReadRecords<ClickEvent>(_eventsPath).ToList();
                events.Add(clickEvent);
                WriteRecords(_eventsPath, events);
            }
        }

        public IEnumerable<ClickEvent> GetClickEvents(string shortCode)
        {
            var url = GetByShortCode(shortCode);
            if (url is null)
            {
                return Enumerable.Empty<ClickEvent>();
            }
            return ReadRecords<ClickEvent>(_eventsPath).Where(x => x.UrlId == url.Id);
        }

        public bool ExistsByShortCode(string shortCode)
        {
            return GetAll().Any(x =>
                string.Equals(x.ShortCode, shortCode, StringComparison.OrdinalIgnoreCase) ||
                x.RedirectAliases.Any(alias => string.Equals(alias, shortCode, StringComparison.OrdinalIgnoreCase)));
        }

        private static IEnumerable<T> ReadRecords<T>(string path)
        {
            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                return Enumerable.Empty<T>();
            }
            return JsonSerializer.Deserialize<IEnumerable<T>>(json) ?? Enumerable.Empty<T>();
        }

        private static string FindProjectRoot()
        {
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (current is not null)
            {
                if (File.Exists(Path.Combine(current.FullName, "server.csproj")))
                {
                    return current.FullName;
                }
                current = current.Parent;
            }

            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        }

        private void MigrateExistingDataIfNeeded()
        {
            var legacyFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "data"));
            var legacyUrls = Path.Combine(legacyFolder, "urls.json");
            var legacyEvents = Path.Combine(legacyFolder, "click-events.json");

            if (Directory.Exists(legacyFolder) && File.Exists(legacyUrls) && !File.Exists(_urlsPath))
            {
                File.Copy(legacyUrls, _urlsPath, overwrite: true);
            }

            if (Directory.Exists(legacyFolder) && File.Exists(legacyEvents) && !File.Exists(_eventsPath))
            {
                File.Copy(legacyEvents, _eventsPath, overwrite: true);
            }
        }

        private IEnumerable<UrlRecord> ReadRecords(string path)
        {
            return ReadRecords<UrlRecord>(path);
        }

        private static void WriteRecords<T>(string path, IEnumerable<T> records)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static void EnsureFile(string path)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[]");
            }
        }
    }
}
