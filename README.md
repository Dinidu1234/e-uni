# KickBlastRealtimeUI

## How to open
1. Open Visual Studio 2022 (17.8+ recommended).
2. Open `KickBlastRealtimeUI.sln`.
3. Ensure startup project is **KickBlastRealtimeUI**.

## How to run
1. Select Debug / Any CPU.
2. Press F5 or Start.
3. App builds to `KickBlastRealtimeUI/bin/Debug/net8.0-windows/`.

## Clean / Rebuild steps
1. Build > Clean Solution.
2. Build > Rebuild Solution.

## Delete bin/obj fix
If build artifacts are corrupted:
1. Close Visual Studio.
2. Delete `KickBlastRealtimeUI/bin` and `KickBlastRealtimeUI/obj`.
3. Reopen solution and rebuild.

## Change pricing
1. Open **Settings** page in app.
2. Edit pricing fields.
3. Click **Save Pricing**.
4. Values are written to `KickBlastRealtimeUI/appsettings.json` and used immediately.

## Reset database
SQLite database path:
`KickBlastRealtimeUI/bin/Debug/net8.0-windows/Data/kickblast_realtime.db`

To reset:
1. Close app.
2. Delete the DB file above.
3. Run app again. `EnsureCreated()` and seeding run on first startup.
