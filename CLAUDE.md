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
- ASP.NET Core Identity — partially scaffolded (Login, Register, Logout, ForgotPassword, AccessDenied, Manage/Index are in `Areas/Identity/`)
- Entity Framework Core 10 with SQLite (`app.db` at project root)
- Default culture: `es-EC` — currency symbol `$` (Ecuador uses USD)
- Bootstrap + jQuery Validation (via `wwwroot/lib/`)

## Architecture

The app follows standard ASP.NET Core MVC. `Program.cs` wires up EF Core, Identity (with `SpanishIdentityErrorDescriber`), localization (`es-EC` only), and static files. No API layer — everything is server-rendered Razor views. In development, `DbSeeder.SeedAsync` runs on startup to populate demo users, assets, and wishlist items.

### Controllers

- **`AssetsController`** — full CRUD for retro game assets. `Index` supports search, platform/condition filtering, and pagination (12 per page). Create/Edit handle optional image upload (`wwwroot/uploads/assets/`, JPG/PNG/WEBP, max 5 MB). Delete is a soft-delete (`IsActive = false`). Authorization: `[AllowAnonymous]` on Index/Details, `[Authorize]` on write actions; ownership checked with `asset.OwnerId != currentUserId` before edit/delete.
- **`WishlistController`** — `[Authorize]` on all actions. Add prevents saving own assets or duplicates. Remove verifies the item belongs to the current user.
- **`CollectorsController`** — `[AllowAnonymous]`. Fetches users who have active assets, groups their top 3 platforms, orders by asset count.
- **`UsersController`** — `[AllowAnonymous]`. Public profile page showing user info and their 12 most-recent active assets.

### Domain model relationships

```
ApplicationUser (IdentityUser)
  ├── Assets[]          (one-to-many, OwnerId FK)
  ├── WishlistItems[]   (one-to-many, UserId FK)
  ├── TradeOffers[]     (one-to-many, OwnerId FK)  ← user who posted the offer
  ├── ReviewsGiven[]    (one-to-many, FromUserId FK)
  └── ReviewsReceived[] (one-to-many, ToUserId FK)

Asset
  ├── TradeOffers[]     (one-to-many, AssetId FK)
  └── WishlistItems[]   (one-to-many, AssetId FK)
```

All FKs use `DeleteBehavior.Restrict` in `ApplicationDbContext.OnModelCreating` — SQLite does not support multiple cascade paths.

**`TradeOffer.OwnerId`** is the user who *posted* the offer (the asset owner), not the user who responds.

### ViewModels

`AssetFormViewModel` is shared for Create and Edit. On Edit, `Id` is non-null and `CurrentImageUrl` holds the existing image path so the view can show it while `ImageFile` remains optional.

### View components

`WishlistCountViewComponent` (`Components/WishlistCountViewComponent.cs`) renders the wishlist badge count in the navbar. Its view is at `Views/Shared/Components/WishlistCount/Default.cshtml`. It returns empty content for anonymous users.

### Helpers and services

- `Helpers/EnumExtensions.cs` — `GetDisplayName()` extension reads the `[Display(Name)]` attribute from enum members. Use this (not `.ToString()`) when rendering enums in views.
- `Services/SpanishIdentityErrorDescriber.cs` — overrides all Identity validation error messages in Spanish; registered in `Program.cs`.

### Identity

- `ApplicationUser` extends `IdentityUser` with: `DisplayName`, `City`, `Country`, `AvatarUrl`, `ReputationScore`, `CreatedAt`.
- Scaffolded pages live in `Areas/Identity/Pages/Account/`: Login, Register, Logout, ForgotPassword, AccessDenied, and Manage/Index.
- Email confirmation is **disabled** (`RequireConfirmedAccount = false`) — re-enable before production.

## Database conventions

- All FK relationships use `DeleteBehavior.Restrict`.
- Soft delete on `Asset` via `IsActive` (`bool`, default `true`) — never hard-delete Asset rows; always filter `IsActive == true` in queries.
- Migrations live in `Data/Migrations/`.
- **Decimal fields must use `[Column(TypeName = "TEXT")]`** — SQLite has no native decimal type; `Asset.EstimatedValue` and `TradeOffer.Price` follow this pattern. Any new monetary field must do the same.

## Model field constraints

- `Asset.Year`: 1970–2010 (retro range enforced by `[Range]`)
- `Review.Rating`: 1–5 integer

## Enums

All enums in `Models/Enums.cs` carry `[Display(Name = "...")]` attributes in Spanish. Always call `.GetDisplayName()` (from `Helpers/EnumExtensions`) instead of `.ToString()` when rendering enum values in views.

Enums: `Platform` (NES → Other), `Region` (NTSC_US/PAL/NTSC_JP), `Condition` (Mint/Good/Fair/Poor), `TradeType` (Sale/Trade/Both), `TradeStatus` (Active/Pending/Closed/Cancelled).

## Not yet implemented

`TradeOffer` and `Review` models exist but have no controllers or views.
