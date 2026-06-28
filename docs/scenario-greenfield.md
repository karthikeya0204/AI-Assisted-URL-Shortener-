# Greenfield Scenario

## Goal
Build the initial end-to-end URL shortener flow from scratch.

## Implementation
- Created URL creation endpoint (`POST /api/urls`).
- Implemented redirect endpoint (`GET /{code}`).
- Added JSON persistence for URL storage.
- Added analytics logging for successful redirects.

## Validation
- Verified URL creation with valid HTTP/HTTPS input.
- Confirmed redirect works for active, non-expired links.
- Confirmed analytics increment on each redirect.
