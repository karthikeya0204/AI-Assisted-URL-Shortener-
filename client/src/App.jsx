import { useEffect, useState } from 'react';

const apiUrl = '/api/urls';

function formatDateTime(value) {
  if (!value) return 'Never';
  return new Date(value).toLocaleString();
}

function toDateTimeLocalValue(value) {
  if (!value) return '';
  const date = new Date(value);
  const offset = date.getTimezoneOffset();
  const local = new Date(date.getTime() - offset * 60000);
  return local.toISOString().slice(0, 16);
}

function App() {
  const [originalUrl, setOriginalUrl] = useState('');
  const [customAlias, setCustomAlias] = useState('');
  const [expiresAt, setExpiresAt] = useState('');
  const [urls, setUrls] = useState([]);
  const [message, setMessage] = useState(null);
  const [selectedUrl, setSelectedUrl] = useState(null);
  const [analytics, setAnalytics] = useState(null);
  const [editAlias, setEditAlias] = useState('');
  const [editExpiresAt, setEditExpiresAt] = useState('');
  const [editActive, setEditActive] = useState(true);
  const [isEditing, setIsEditing] = useState(false);

  const loadUrls = async () => {
    try {
      const response = await fetch(apiUrl);
      const data = await response.json();
      setUrls(data);
    } catch {
      setMessage('Unable to load URLs.');
    }
  };

  useEffect(() => {
    loadUrls();
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

  const startEdit = (url) => {
    setSelectedUrl(url);
    setAnalytics(null);
    setEditAlias(url.shortCode);
    setEditExpiresAt(toDateTimeLocalValue(url.expiresAt));
    setEditActive(url.isActive);
    setIsEditing(true);
    setMessage(null);
  };

  const viewAnalytics = async (url) => {
    setSelectedUrl(url);
    setAnalytics(null);
    setIsEditing(false);
    setMessage(null);

    try {
      const response = await fetch(`${apiUrl}/${encodeURIComponent(url.shortCode)}/analytics`);
      const data = await response.json();
      setAnalytics(data);
    } catch {
      setMessage('Unable to load analytics.');
    }
  };

  const saveEdit = async (event) => {
    event.preventDefault();

    if (!selectedUrl) return;

    const body = {
      customAlias: editAlias.trim() || undefined,
      expiresAt: editExpiresAt ? new Date(editExpiresAt).toISOString() : null,
      isActive: editActive,
    };

    try {
      const response = await fetch(`${apiUrl}/${encodeURIComponent(selectedUrl.shortCode)}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        const error = await response.json();
        setMessage(error?.error || 'Failed to update URL');
        return;
      }

      const updated = await response.json();
      setUrls((current) => current.map((item) => (item.id === updated.id ? updated : item)));
      setSelectedUrl(updated);
      setEditAlias(updated.shortCode);
      setEditExpiresAt(toDateTimeLocalValue(updated.expiresAt));
      setEditActive(updated.isActive);
      setIsEditing(false);
      await loadUrls();
      setMessage('URL updated successfully.');
    } catch {
      setMessage('Unable to update URL.');
    }
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
                <th>Actions</th>
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
                  <td>{formatDateTime(url.expiresAt)}</td>
                  <td>{url.isActive ? 'Yes' : 'No'}</td>
                  <td>
                    <div className="action-buttons">
                      <button type="button" className="secondary" onClick={() => startEdit(url)}>
                        Edit
                      </button>
                      <button type="button" className="secondary" onClick={() => viewAnalytics(url)}>
                        Analytics
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      {selectedUrl && (
        <section className="details-panel">
          <h2>Selected URL</h2>
          <p>
            <strong>Short code:</strong> {selectedUrl.shortCode}
          </p>
          <p>
            <strong>Original URL:</strong> {selectedUrl.originalUrl}
          </p>

          {analytics ? (
            <div className="analytics-card">
              <h3>Analytics</h3>
              <p>
                <strong>Total clicks:</strong> {analytics.totalClicks}
              </p>
              <p>
                <strong>Active:</strong> {analytics.isActive ? 'Yes' : 'No'}
              </p>
              <p>
                <strong>Expires at:</strong> {formatDateTime(analytics.expiresAt)}
              </p>
            </div>
          ) : null}

          {isEditing ? (
            <form onSubmit={saveEdit} className="edit-form">
              <label>
                Alias
                <input value={editAlias} onChange={(e) => setEditAlias(e.target.value)} placeholder="new-alias" />
              </label>
              <label>
                Expires At
                <input type="datetime-local" value={editExpiresAt} onChange={(e) => setEditExpiresAt(e.target.value)} />
              </label>
              <label className="checkbox-row">
                <input type="checkbox" checked={editActive} onChange={(e) => setEditActive(e.target.checked)} />
                Active
              </label>
              <div className="action-buttons">
                <button type="submit">Save Changes</button>
                <button type="button" className="secondary" onClick={() => setIsEditing(false)}>
                  Cancel
                </button>
              </div>
            </form>
          ) : (
            <div className="action-buttons">
              <button type="button" onClick={() => startEdit(selectedUrl)}>
                Edit URL
              </button>
              <button type="button" className="secondary" onClick={() => viewAnalytics(selectedUrl)}>
                View Analytics
              </button>
            </div>
          )}
        </section>
      )}
    </div>
  );
}

export default App;
