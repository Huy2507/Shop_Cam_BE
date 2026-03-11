# Shop Cam Backend (Shop_Cam_BE)

Base backend solution using .NET 8 and a Clean Architecture style layout

## Structure

- `src/Domain` – core domain models and interfaces (empty scaffold).
- `src/Application` – use cases, DTOs, validators, etc. (empty scaffold).
- `src/Infrastructure` – persistence, external services, implementations (empty scaffold).
- `src/Web` – ASP.NET Core Web API project (currently a minimal API with Swagger).

## Getting started

From `Shop_Cam_BE`:

```bash
dotnet build
cd src/Web
dotnet run
```

Then open `https://localhost:62143` (or the URL shown in the console) to see the Swagger UI.
