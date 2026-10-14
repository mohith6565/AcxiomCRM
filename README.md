# AcxiomCRM — Technical Round Starter Project

This is an ASP.NET Core MVC starter implementation based on the supplied 19-page AcxiomCRM specification.

## Included
- ASP.NET Core Identity login/register/logout, password policy and lockout configuration
- Roles seeded: `Admin`, `Manager`, `SalesExecutive`
- SQLite + Entity Framework Core
- Dashboard KPI cards and Chart.js opportunity-stage chart
- Customer create/read/update, search, duplicate validation and role/owner scoping
- Lead create/read/update, search and validation
- Opportunity creation and listing, amount/probability/date rules
- Follow-up scheduling and listing with date validation
- Customer REST API: `GET /api/customers`, `GET /api/customers/{id}`, `POST /api/customers`
- Audit records for key customer, lead, opportunity and follow-up operations
- Responsive Bootstrap layout

## Requirements
- .NET 8 SDK
- Internet access for NuGet restore and Bootstrap/Chart.js CDN assets

## Run locally
```bash
dotnet restore
dotnet run
```
Open the HTTPS URL printed by `dotnet run`. Register the first user through the Register page.

## Important setup notes
1. The app creates a local SQLite database using `EnsureCreated()` on first startup. For a production or evaluated deployment, replace this with EF Core migrations (`dotnet ef migrations add InitialCreate`, then `dotnet ef database update`).
2. New users are not automatically granted an elevated role. Assign roles using a controlled admin-only user-management workflow before testing role-specific scenarios. For a local demo only, you can seed a known test account and role in a development-only seeder; do not ship default credentials.
3. `CookieSecurePolicy.Always` expects HTTPS. For local HTTP-only testing, use HTTPS or temporarily change the setting only in Development.
4. The project is a functional starter, not a completed production security review. Complete the user/role administration screens, audit log viewer, report filters/export, opportunity update/delete, follow-up rescheduling/completion, lead conversion transaction, rate limiting, and comprehensive automated tests before claiming full compliance with every item in the source specification.
5. Configure a production database, secret management, HTTPS, backups, logging/monitoring, migrations and least-privilege hosting before deployment.

## Main project structure
```text
Controllers/
  Api/CustomersApiController.cs
  CustomersController.cs
  FollowUpsController.cs
  HomeController.cs
  LeadsController.cs
  OpportunitiesController.cs
Data/ApplicationDbContext.cs
Models/
  ApplicationUser.cs
  Customer.cs
  Lead.cs
  Opportunity.cs
  FollowUp.cs
  AuditLog.cs
Services/AuditService.cs
Views/
wwwroot/css/site.css
Program.cs
```

## Specification coverage still to finish
- User/role management screens and permission matrix enforcement across every action
- Complete CRUD and deletion/deactivation workflows for all entities
- Lead-to-customer/opportunity conversion workflow
- Audit log search/filter UI and hardened append-only storage
- All report pages, filters, pagination and exports
- More comprehensive dashboard scoping for each role and monthly sales chart
- API endpoints for leads and opportunities, API authentication strategy, throttling and API documentation
- Automated tests for validation, authorization, workflows and tampered requests
