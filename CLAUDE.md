# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# GameVault — Project Reference

## Project

**GameVault** is a platform for retro video game collectors.

- **Team:** Nexus Asset Labs — Flavio Ibujes, Elian Hidalgo, David Morales
- **Course:** Programación IV — instructor PhD(c) Luis Fernando Aguas Bucheli

## Commands

```powershell
# Run the app (HTTP on localhost:5236, HTTPS on localhost:7160)
dotnet run

# Apply pending EF migrations to app.db
dotnet ef database update

# Add a new migration after changing models
dotnet ef migrations add <MigrationName> --output-dir Data/Migrations

# Build only
dotnet build

# Restore packages
dotnet restore
```

There are no automated tests in this project yet.

## Language conventions

- All **user-facing text** (views, labels, buttons, validation messages, error pages, navigation, emails) must be in **Spanish (es-EC)**.
- All **code** (class names, properties, methods, variables, file names) and **comments** must be in **English**.
- Every model property must have `[Display(Name = "...")]` in Spanish.
- Every validation attribute (`[Required]`, `[StringLength]`, `[Range]`) must include a Spanish `ErrorMessage`.

## Stack

- ASP.NET Core MVC 10
- ASP.NET Core Identity — UI served from the package (not scaffolded into the project)
- Entity Framework Core 10 with SQLite (`app.db` at project root)
- Default culture: `es-EC` — currency symbol `$` (Ecuador uses USD)
- Bootstrap + jQuery Validation (via `wwwroot/lib/`)

## Architecture

The app follows standard ASP.NET Core MVC. `Program.cs` wires up EF Core, Identity, localization (`es-EC` only), and static files. No API layer exists — everything is server-rendered Razor views.

### Domain model relationships

```
ApplicationUser (IdentityUser)
  ├── Assets[]          (one-to-many, OwnerId FK)
  ├── TradeOffers[]     (one-to-many, OwnerId FK)  ← user who posted the offer
  ├── ReviewsGiven[]    (one-to-many, FromUserId FK)
  └── ReviewsReceived[] (one-to-many, ToUserId FK)

Asset
  └── TradeOffers[]     (one-to-many, AssetId FK)
```

All FKs are configured with `DeleteBehavior.Restrict` in `ApplicationDbContext.OnModelCreating` because SQLite does not support multiple cascade paths.

**`TradeOffer.OwnerId`** is the user who *posted* the trade offer (typically the asset owner wanting to sell or trade). This is distinct from the user who responds to the offer.

### Current implementation state

Only `HomeController` exists (Index, Privacy, Error). No CRUD controllers for Asset, TradeOffer, or Review have been implemented yet.

### Identity

- `ApplicationUser` extends `IdentityUser` with: `DisplayName`, `City`, `Country`, `AvatarUrl`, `ReputationScore`, `CreatedAt`.
- Identity UI pages (register, login, account management) are served from the `Microsoft.AspNetCore.Identity.UI` package. The only Identity file in this repo is `Areas/Identity/Pages/_ViewStart.cshtml`, which pins those pages to the shared layout.
- Email confirmation is **disabled in development** (`RequireConfirmedAccount = false`) — re-enable before production.

## Database conventions

- All FK relationships use `DeleteBehavior.Restrict`.
- Soft delete on `Asset` via `IsActive` (`bool`, default `true`) — do not hard-delete Asset rows; always filter `IsActive == true` in queries.
- Migrations live in `Data/Migrations/`.
- **Decimal fields must use `[Column(TypeName = "TEXT")]`** — SQLite has no native decimal type; `Asset.EstimatedValue` and `TradeOffer.Price` both follow this pattern. Any new monetary field must do the same.

## Model field constraints

- `Asset.Year`: 1970–2010 (retro range enforced by `[Range]`)
- `Review.Rating`: 1–5 integer

## Enums

All enums in `Models/Enums.cs` carry `[Display(Name = "...")]` attributes in Spanish. When rendering enum values in views, use a helper or `Html.DisplayNameFor` so the Spanish label is shown instead of the member name.

Enums: `Platform` (NES → Other), `Region` (NTSC_US/PAL/NTSC_JP), `Condition` (Mint/Good/Fair/Poor), `TradeType` (Sale/Trade/Both), `TradeStatus` (Active/Pending/Closed/Cancelled).
