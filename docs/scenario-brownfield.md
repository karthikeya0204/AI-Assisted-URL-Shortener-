# Brownfield Scenario

## Enhancement
Added custom alias support and expiry date support.

## Changes
- `CreateUrlRequest` supports `customAlias`.
- `UpdateUrlRequest` supports `expiresAt` and `isActive`.
- `UrlService` enforces alias uniqueness.
- Redirect blocking occurs when links expire or are inactive.

## Validation
- Confirmed duplicate alias is rejected.
- Confirmed expired links do not redirect.
- Confirmed active flag prevents redirect when set to false.
