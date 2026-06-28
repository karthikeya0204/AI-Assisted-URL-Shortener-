# Architecture Memo

## Overview
This prototype implements a URL shortener with an ASP.NET Core Web API backend and a minimal React frontend.

## Components
- `server/` - backend service
  - `Controllers/` - API controllers for URL creation, retrieval, update, analytics, and redirect
  - `Services/` - business logic and validation
  - `Repositories/` - JSON persistence layer for URLs and click events
  - `DTOs/` - request and response models
  - `Middleware/` - centralized exception handling
  - `data/` - persistence storage for `urls.json` and `click-events.json`
- `client/` - frontend app built with React and Vite
  - `src/` - components, service layer, and styling

## Key Decisions
- JSON-backed file persistence was chosen for rapid prototype delivery and ease of review.
- Layered architecture separates concerns and makes the application easier to test.
- Custom alias support and expiry handling satisfy the brownfield enhancement requirement.
- Simple click event logging supports analytics without requiring a database.

## Trade-offs
- JSON file storage is not production-ready and has limited concurrency support.
- No authentication or rate limiting is implemented in this MVP.
- Redirect reliability is basic but sufficient for prototype validation.
