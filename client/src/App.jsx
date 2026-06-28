import { useEffect, useState } from 'react';

const apiUrl = '/api/urls';

function App() {
  const [originalUrl, setOriginalUrl] = useState('');
  const [customAlias, setCustomAlias] = useState('');
  const [expiresAt, setExpiresAt] = useState('');
  const [urls, setUrls] = useState([]);
  const [message, setMessage] = useState(null);

  useEffect(() => {
    fetch(apiUrl)
      .then((res) => res.json())
      .then(setUrls)
      .catch(() => setMessage('Unable to load URLs.'));
  }, []);

  const createUrl = async (event) => {
    event.preventDefault();

    const body = {
      originalUrl,
      customAlias: customAlias || undefined,
      expiresAt: expiresAt || undefined,
    };

    const response = await fetch(apiUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const error = await response.json();
      setMessage(error?.error || 'Failed to create URL');
      return;
    }

    const data = await response.json();
    setUrls((current) => [data, ...current]);
    setOriginalUrl('');
    setCustomAlias('');
    setExpiresAt('');
    setMessage('Short URL created successfully.');
  };

  return (
    <div className="app-container">
      <h1>AI-Assisted URL Shortener</h1>
      <form onSubmit={createUrl} className="form">
        <label>
          Original URL
          <input value={originalUrl} onChange={(e) => setOriginalUrl(e.target.value)} placeholder="https://example.com" required />
        </label>
        <label>
          Custom Alias (optional)
          <input value={customAlias} onChange={(e) => setCustomAlias(e.target.value)} placeholder="myalias" />
        </label>
        <label>
          Expires At (optional)
          <input type="datetime-local" value={expiresAt} onChange={(e) => setExpiresAt(e.target.value)} />
        </label>
        <button type="submit">Create Short URL</button>
      </form>

      {message && <div className="message">{message}</div>}

      <section className="list-section">
        <h2>Created URLs</h2>
        {urls.length === 0 ? (
          <p>No URLs created yet.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Short URL</th>
                <th>Original URL</th>
                <th>Clicks</th>
                <th>Expires At</th>
                <th>Active</th>
              </tr>
            </thead>
            <tbody>
              {urls.map((url) => (
                <tr key={url.id}>
                  <td>
                    <a href={url.shortUrl} target="_blank" rel="noreferrer">
                      {url.shortUrl}
                    </a>
                  </td>
                  <td>{url.originalUrl}</td>
                  <td>{url.clickCount}</td>
                  <td>{url.expiresAt ? new Date(url.expiresAt).toLocaleString() : 'Never'}</td>
                  <td>{url.isActive ? 'Yes' : 'No'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </div>
  );
}

export default App;
