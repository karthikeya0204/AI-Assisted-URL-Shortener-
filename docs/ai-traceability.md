# AI Traceability

## Task
Build an AI-assisted MVP for an URL shortener with backend, frontend, analytics, and documentation.

## AI Usage
- Used AI to draft architecture and component structure.
- Generated backend scaffolding and frontend scaffold.
- Created project documentation and test guidance.

## Owner Decisions
- Chose ASP.NET Core Web API for backend clarity and strong typed contracts.
- Used JSON file persistence for prototype speed and simplicity.
- Added custom alias, expiry date, and analytics support to satisfy the brief.

## Accepted Outputs
- Backend project under `server/`.
- Frontend project under `client/`.
- Documentation under `docs/`.
- Test project for backend logic.

## Rationale
The solution intentionally keeps architecture simple but layered for maintainability and review. JSON persistence is a deliberate prototype trade-off.
