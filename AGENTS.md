# Repository Guidelines

## Project Structure & Module Organization
- Solution: `AspireStarter.sln` (root).
- Backend (ASP.NET Core + .NET Aspire): `AspireStarter/`
  - `AspireStarter.AppHost/` (Aspire orchestration, `Program.cs`, `manifest.json`, `azure.yaml`).
  - `AspireStarter.ApiService/` (API service).
  - `AspireStarter.Web/` (web frontend host + BFF logic).
  - `AspireStarter.ServiceDefaults/` (shared hosting/telemetry defaults).
- Libraries: `MailDev.Client.MailKit/`, `MailDev.Hosting/`.
- React (Next.js 14): `ReactFrontend/react-frontend/`.

## Build, Test, and Development Commands
- Restore/build: `dotnet restore` then `dotnet build AspireStarter.sln -c Debug`.
- Run all (Aspire): `dotnet run --project AspireStarter/AspireStarter.AppHost` — starts orchestrated services.
- Run service: e.g., `dotnet run --project AspireStarter/AspireStarter.Web`.
- Frontend dev: `cd ReactFrontend/react-frontend && npm ci && npm run dev`.
- Format C#: `dotnet format --verify-no-changes` (uses `.editorconfig`).

## Coding Style & Naming Conventions
- Source of truth: `.editorconfig` (4‑space indent, LF, spaces, trailing whitespace trimmed).
- C#: braces required; file‑scoped namespaces; prefer explicit types over `var`; expression‑bodied accessors allowed.
- Naming: PascalCase for types/members; follow `.editorconfig` rules (e.g., private static fields/constants PascalCase).
- Keep one public type per file; organize `using` directives; small, focused classes.

## Testing Guidelines
- Framework: xUnit is recommended. Place tests in `<ProjectName>.Tests/` and add to the solution.
- Naming: files `ClassNameTests.cs`; methods `MethodOrBehavior_Should_ExpectedResult`.
- Run tests: `dotnet test` at repo root.
- Aim for coverage of critical paths (DI wiring, controllers/handlers, clients).

## Commit & Pull Request Guidelines
- Commits: short imperative summary, optional scope. Example: `feat(web): add retry to WeatherApiClient`.
- PRs: clear description, link issues, include screenshots for UI, steps to validate, and highlight any config changes.
- Before opening: ensure `dotnet build` passes, `dotnet format` is clean, and the AppHost/Next.js dev servers run locally.

## Security & Configuration Tips
- App settings: use `appsettings.Development.json` for local non‑secrets; prefer `dotnet user-secrets` for secrets.
- Next.js: use `.env.local` (client‑exposed keys prefixed with `NEXT_PUBLIC_`). Never commit secrets.
