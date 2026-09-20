# Copilot Instructions

## Project Guidelines
- The web UI must remain in Angular and use Bootstrap styles; avoid proposing Razor Pages or server-rendered admin views for this project.

## Authentication and Authorization
- Roles must be defined as follows:
  - **SuperAdmin**: Full site access
  - **Owner**: Access to one or more owned hotels
  - **Admin**: Management and reservations for a single hotel
  - **User**: Customer making reservations

## Unit Testing
- Unit tests must be written for all new features and components.
- Refactor existing code to improve test coverage where necessary.

## Image Storage
- When storing uploaded room images in Docker, use the persistent hotel-images volume and place room images under `wwwroot/hotel-images/rooms` so they survive container restarts.