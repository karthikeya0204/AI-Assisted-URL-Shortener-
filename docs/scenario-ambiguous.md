# Ambiguous Scenario

## Ambiguity
The brief left aspects of reliability and abuse prevention undefined.

## Approach
- Implemented strict URL validation for HTTP/HTTPS schemes.
- Added duplicate alias rejection to prevent collisions.
- Implemented expiry handling and active/inactive state.
- Logged click events to support analytics and diagnose unexpected traffic.

## Assumptions
- Only basic redirect analytics are required.
- No rate limiting or authentication is needed for the MVP.
- JSON file persistence is acceptable for prototype evaluation.
