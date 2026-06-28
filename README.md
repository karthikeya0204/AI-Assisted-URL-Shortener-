# AI-Assisted-URL-Shortener

This repository contains a full MVP for an AI-assisted URL shortener prototype. The implementation includes a backend API, frontend UI, JSON persistence, redirect analytics, and documentation covering architecture and scenarios.

## Structure
- `server/` - ASP.NET Core Web API backend
- `server/data/` - JSON persistence for URLs and click events
- `server/Tests/` - xUnit service tests
- `client/` - React frontend built with Vite
- `docs/` - architecture, scenario, testing, and AI traceability documentation

## Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- Git (optional for repository cloning)

## Backend Setup
1. Clone the repository and navigate to the backend folder:
   ```powershell
   git clone <your-repo-url> <repo-root>
   cd <repo-root>\server
   ```
2. Restore backend dependencies:
   ```powershell
   dotnet restore
   ```
3. Start the backend API:
   ```powershell
   dotnet run
   ```
4. Confirm the backend is running at:
   ```text
   http://localhost:5000
   ```

## Frontend Setup
1. Open a new PowerShell window and navigate to the frontend folder:
   ```powershell
   cd <repo-root>\client
   ```
2. Install frontend dependencies:
   ```powershell
   npm install
   ```
3. Start the frontend dev server:
   ```powershell
   npm run dev
   ```
4. Confirm the frontend is running at:
   ```text
   http://localhost:3000
   ```

## Running Tests
### Backend unit tests
From the `server` folder:
```powershell
dotnet test "AI-Assisted-URL-Shortener.sln"
```

## API Endpoints and Usage
### 1. Create a short URL
- Endpoint: `POST /api/urls`
- Request body:
  ```json
  {
    "originalUrl": "https://example.com/long/path",
    "customAlias": "myalias",
    "expiresAt": "2030-01-01T00:00:00Z"
  }
  ```
- Successful response includes the generated `shortCode` and `shortUrl`.

### 2. Create a short URL with automatic code
If `customAlias` is omitted, the service generates a unique 8-character code.

### 3. List all URLs
- Endpoint: `GET /api/urls`
- Returns all saved shortened URL records.

### 4. Get URL metadata
- Endpoint: `GET /api/urls/{code}`
- Example:
  ```text
  GET /api/urls/testalias
  ```

### 5. Update URL expiry or active status
- Endpoint: `PUT /api/urls/{code}`
- Supported updates:
  - `expiresAt`
  - `isActive`
- Example request body:
  ```json
  {
    "expiresAt": "2030-12-31T23:59:59Z",
    "isActive": false
  }
  ```
- Note: alias update is not supported after creation. To rename an alias, create a new short URL and deactivate the old one.

### 6. View analytics for a short URL
- Endpoint: `GET /api/urls/{code}/analytics`
- Returns click count, expiry, and active status.

### 7. Redirect to the original URL
- Endpoint: `GET /{code}`
- Example:
  ```text
  GET /testalias
  ```
- Redirects the browser to the original destination URL if the short URL is active and not expired.

## Frontend usage
Once both backend and frontend are running:
1. Open `http://localhost:3000` in your browser.
2. Enter the original URL in the form.
3. Optionally provide a `Custom Alias` and `Expires At` value.
4. Click `Create Short URL`.
5. The created URL list shows:
   - short URL
   - original URL
   - click count
   - expiry status
   - active status

## Example feature tests
### Create a short URL
- Use the frontend form or the API `POST /api/urls`
- Verify the returned `shortUrl` and `shortCode`

### Create with custom alias
- Provide `customAlias` in the POST payload
- Verify the returned `shortCode` matches the alias

### Disable a short URL
- Use `PUT /api/urls/{code}` with `{"isActive": false}`
- Confirm `GET /{code}` returns 404 or does not redirect

### Expiry behavior
- Set `expiresAt` to a past date using `PUT /api/urls/{code}`
- Confirm `GET /{code}` no longer redirects

## Notes
- Data is saved in `server/data/urls.json` and `server/data/click-events.json`.
- This prototype is intended for evaluation and review, not production use.
- If you need to change an alias, create a new URL record and deactivate the old alias.

## Documentation
- `docs/architecture-memo.md`
- `docs/scenario-greenfield.md`
- `docs/scenario-brownfield.md`
- `docs/scenario-ambiguous.md`
- `docs/testing-approach.md`
- `docs/ai-traceability.md`
