# FRISL Enterprise Asset Management System (EAMS)

## Overview
ASP.NET Core 8 web application implementing a full-featured Enterprise Asset Management System for FRISL. Combines ASP.NET Core MVC for workflow screens with Blazor Server for the interactive admin dashboard.

## Architecture
- **Framework**: ASP.NET Core 8 MVC + Blazor Server
- **Database**: SQLite via Entity Framework Core 8 (file: `FrislEams.Web/eams.db`)
- **Port**: 5000 (HTTP, 0.0.0.0)
- **Runtime**: .NET 8

## Project Structure
```
FrislEams.Web/
├── Controllers/        # MVC + API controllers
│   └── Api/           # REST API endpoints
├── Components/         # Blazor components
│   └── Pages/         # Dashboard.razor (Blazor admin dashboard)
├── Data/              # EF Core DbContext + seed data
├── Domain/            # Enums (AssetStatus, RoleName)
├── Models/            # Entities and ViewModels
├── Services/          # Business logic services
├── Views/             # Razor MVC views
└── wwwroot/           # Static files
```

## Key Features
- Asset registration with auto tag-code generation (`FRISL-YEAR-CAT-DEPT-SEQ`)
- RFID uniqueness and master registration
- Two-leg assignment workflow (initiation + receipt confirmation)
- Repair/replacement/discard workflow
- Loan workflow (internal/external) with external exit-grant creation
- RFID door scan monitoring with unauthorized-exit alerts
- Asset request intake
- Immutable asset lifecycle history
- Admin dashboard metrics (Blazor Server interactive)
- Audit framework (sessions, results, variance)
- Reporting engine (filtered asset report + CSV export)
- Depreciation report (straight-line) with CSV export
- OpenAPI/Swagger (development only, at `/swagger`)
- Integration webhooks/queue processing for procurement and RFID batch ingestion
- Lightweight RBAC guard (`X-Role` header or `?role=` query param)

## Running the Application
The workflow command: `cd FrislEams.Web && DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet run`

The app starts on port 5000. The SQLite database is auto-created and seeded on first run.

## API Endpoints
- `GET /api/assets`
- `POST /api/assets/register`
- `POST /api/assets/{id}/status`
- `POST /api/monitoring/rfid-scan`
- `GET /api/monitoring/events`
- `GET /api/monitoring/assets-report`
- `POST /api/integration/procurement-sync`
- `POST /api/integration/rfid-batch`
- `GET /api/integration/events`

## Deployment
- Target: autoscale
- Build: `cd FrislEams.Web && dotnet publish -c Release -o out`
- Run: `./FrislEams.Web/out/FrislEams.Web --urls http://0.0.0.0:5000`
