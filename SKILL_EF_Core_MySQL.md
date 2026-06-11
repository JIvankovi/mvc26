# SKILL: Reset & Sync EF Core + MySQL Database (projekt Workspace)

**Workspace-scoped** quick reference for resetting migrations and syncing the `projekt_db` schema across machines.

## Use When
- Database schema drifts from current models
- Multiple machines need synchronized schema  
- Migration history is corrupted or conflicted
- Starting fresh with a clean baseline migration

---

## Reset & Rebuild Workflow (5 Steps)

1. **List existing migrations**
   ```powershell
   dotnet ef migrations list
   ```

2. **Remove all old migrations** (repeat until clean)
   ```powershell
   dotnet ef migrations remove --force
   ```

3. **Drop the database**
   ```powershell
   dotnet ef database drop --force
   ```

4. **Create new baseline migration** (use timestamp-friendly name)
   ```powershell
   dotnet ef migrations add InitialCreate
   ```

5. **Apply migration to database**
   ```powershell
   dotnet ef database update
   ```

---

## Sync Across Machines

**On source machine (after reset above):**
```powershell
git add projekt/Migrations/
git commit -m "Reset migrations: clean baseline for projekt_db"
git push
```

**On target machines:**
```powershell
git pull
dotnet ef database update
```

---

## Validation Checklist

- [ ] `dotnet ef migrations list` shows single migration (no pending)
- [ ] MySQL `projekt_db` exists with all entity tables (`Devices`, `Laboratories`, `Technicians`, `Calibrations`, `DeviceLocations`)
- [ ] `__EFMigrationsHistory` table contains one entry
- [ ] Application builds without model/context errors
- [ ] Schema matches current entity models
- [ ] All machines show same migration in `__EFMigrationsHistory`

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "dotnet-ef: command not found" | `dotnet tool install --global dotnet-ef` |
| "No database provider configured" | Check `Program.cs` DbContext setup; verify connection string in `appsettings.json` |
| "Database drop failed" | Ensure no active connections; check MySQL permissions |
| "Models still don't match DB after update" | Rebuild project; verify all entity `DbSet<>` properties exist in `ApplicationDbContext` |

---

## Workspace Context

- **Project**: `projekt/projekt.csproj` (.NET 9)
- **Database**: `projekt_db` (local MySQL)
- **DbContext**: `ApplicationDbContext` (Data/ApplicationDbContext.cs)
- **Entities**: Device, Laboratory, Technician, Calibration, DeviceLocation
- **Connection**: `DefaultConnection` (appsettings.json)
- **Baseline Migration**: `20260521141911_InitialCreate` (timestamp may vary)

---

## When to Use This vs. Incremental Migrations

**Use this skill (reset)**: Schema conflicts, team desync, migration errors  
**Use incremental**: Adding new properties to models, normal development flow (`dotnet ef migrations add MyFeatureName` → `dotnet ef database update`)
