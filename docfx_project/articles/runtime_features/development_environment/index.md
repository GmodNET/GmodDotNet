---
uid: runtime_features_development_environment
title: "Development environment"
---

# Development environment

Gmod.NET Runtime can run in so called Development environment. If Development environment is on Gmod.NET Runtime can load modules from any location, not only `garrysmod/lua/bin/Modules` folder.
This can improve one's inner dev loop performance since modules can be loaded directly from the .NET project output folder.

Development environment is on if game's or server's process environment variable `DOTNET_ENVIRONMENT` is set to `Development`. 
[This is a common way to enable Development environment in .NET world.](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-5.0)

To set an environment variable for a game process you need to start it from a command line.
Here is an example using Powershell Core on Windows:
```powershell
cd "C:\Program Files (x86)\Steam\steamapps\common\GarrysMod"
$env:DOTNET_ENVIRONMENT='Development'
.\bin\win64\gmod.exe
```

There is also an examle of starting Garry's Mod dedicated server with Gmod.NET in Development environment on Linux using bash:
```bash
cd ~/steam/steamapps/common/GarrysModDS
export DOTNET_ENVIRONMENT="Development"
./srcds_run_x64
```

> [!WARNING]
> Running under Development environment is less secure since Lua scripts can try to load .NET components from anywhere on your PC. 
> Use only trusted Lua scripts and DO NOT join untrusted servers as a client while Development environment is on.