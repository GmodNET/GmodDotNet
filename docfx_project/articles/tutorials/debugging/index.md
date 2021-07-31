---
uid: tutorial_debugging
title: "Debugging and inner dev loop"
---

# Debugging and inner dev loop

Development, debugging, and testing of the Gmod.NET module can be a long complex process. This tutorial will teach you how to improve your inner dev loop experience by skipping coping of build artifacts and use .NET debuggers with Gmod.NET.

## Development environment

While module development, copying build artifacts to `garrysmod/lua/bin/Modules/ModuleName` after each code update may be inefficient and irritating. To address this issue, Gmod.NET Runtime introduced a feature called [Development environment](xref:runtime_features_development_environment). While Development environment is on, Gmod.NET Runtime can load modules from any location on computer by its absolute path. For example, one can load modules directly from the output directory of the C# project. (Note that module still have to be accompined by its `.deps.json` file in order for runtime to resolve dependencies, .NET SDK will automatically generate this file for you for every build.)

Since Gmod.NET modules are loaded via Lua scripts, enabling Development environment by default could cause additional security risks, especially in the case of client with Gmod.NET Runtime joining an untrusted server. Thus, Development environment should be explicitly enabled by users by setting `DOTNET_ENVIRONMENT` environment variable value to `Development` for game or dedicated server process.

Here is how you can enable and use Development environment on Windows:

1. One of the most convenient way to enable Development environment is to use command line interface. We will be using Windows PowerShell.

2. Open PowerShell window. Navigate to Garry's Mod root folder by using `cd` command (like `cd 'C:\Program Files (x86)\Steam\steamapps\common\GarrysMod\'`). Set environment variable by `$env:DOTNET_ENVIRONMENT='Development'` command (note that this is a PowerShell specific syntax for setting environment variables, if you are using other command line shell, like bash on Linux, use corresponding syntax instead). Start Garry's Mod by `.\bin\win64\gmod.exe` command.
[![A screenshot of PowerShell windows with commands above](images/powershell.png)](images/powershell.png)

3. In started Garry's Mod instance, start a new game and load Gmod.NET (by running `lua_run require("dotnet")` in game console, for example). Gmod.NET will inform you that it is running in Development environment by printing a warning message to the game console:
[![A screenshot of Garry's Mod game console with loaded Gmod.NET and warning message](images/gmod1.png)](images/gmod1.png)

4. Now we can load module by its absolute path, something like `lua_run dotnet.load([[C:\Users\glebc\source\repos\TestModule\bin\Debug\net5.0\TestModule.dll]])`.
[![A screenshot of Garry's Mod game console with Gmod.NET module loaded by its absolute path](images/gmod2.png)](images/gmod2.png)

5. To unload module, which which was loaded by absolute path, one should also use module's full path, like `lua_run dotnet.unload([[C:\Users\glebc\source\repos\TestModule\bin\Debug\net5.0\TestModule.dll]])`.
[![A screenshot of Garry's Mod game console with unloaded Gmod.NET module](images/gmod3.png)](images/gmod3.png)

## Debugging Gmod.NET modules