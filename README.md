[![Nuget](https://img.shields.io/nuget/v/GmodNET.API?color=Blue&style=for-the-badge)](https://www.nuget.org/packages/GmodNET.API/) [![Downloads](https://img.shields.io/nuget/dt/GmodNET.API?color=green&style=for-the-badge)](https://www.nuget.org/packages/GmodNET.API/) [![Discord Server](https://img.shields.io/discord/632622505848471554?label=Our%20Discord&style=for-the-badge)](https://discord.gg/9bP8nMT)

# Gmod.NET
[![Current Runtime](https://img.shields.io/badge/Current%20Runtime-0.3.1-green?style=flat-square)](https://github.com/GlebChili/GmodDotNet/wiki/GmodNET-Runtime-and-GmodNET.API-version-correspondence#gmodnet-and-gmodnetapi) [![Current API](https://img.shields.io/badge/Current%20API-0.3.1-green?style=flat-square)](https://github.com/GlebChili/GmodDotNet/wiki/GmodNET-Runtime-and-GmodNET.API-version-correspondence#gmodnet-and-gmodnetapi)

Cross-platform .NET Module/Plugin platform for Garry's Mod powered by [__.NET Core__](https://dotnet.microsoft.com/).

## About

Gmod.NET is Garry's Mod Module/Plugin loader for C#
and other .NET languages which runs across all platforms (Windows,
Linux, MacOs). Gmod.NET allows you to develop Garry's Mod extensions without
need to close or reload your game or server.

## Similar projects

Check out [gmod_csModuleLoader](https://github.com/dedady157/gmod_csModuleLoader) by [Bailey Drahoss](https://github.com/dedady157).

## Current features

GmodNET provides basic functionality to write Garry's Mod modules in C# or any other CIL-compiled language as [__.NET Core 3.0__](https://dotnet.microsoft.com/) class libraries. For more information on modules and API check out [project's wiki](https://github.com/GlebChili/GmodDotNet/wiki). Only `x86_64` version of Garry's Mod is currently supported.

## Need help?

Check out our [wiki](https://github.com/GlebChili/GmodDotNet/wiki) or join our [discord server](https://discord.gg/9bP8nMT).

## Building and contributing

Gmod.NET is subdivided into two subprojects. Garry's Mod binary native module is
contained in __gm_dotnet_native__ folder. It is written in __C++__ and uses
__CMake__ as its build (prebuild) system.

Managed part is an msbuild solution developed against `netcoreapp3.0` specification.

## Installation and usage

1) Download latest build from the project's [releases page](https://github.com/GlebChili/GmodDotNet/releases).

2) Unpack archive to the `%GARRYS_MOD_ROOT_FOLDER%garrysmod/lua/bin/` folder.

3) Create a `Modules` folder inside `garrysmod/lua/bin/`.

4) Place your .NET module, ...deps.json file, and all dependencies in `Modules/%exact_name_of_the_module_without_dll/` folder.

5) Start the game and type `lua_run require("dotnet")` in console (type `lua_run require("dotnet")` to load GmodNET client-side)

6) Use `gmod_net_load_all` (`gmod_net_load_all_cl` for client-side) console command to load all managed modules and `gmod_net_unload_all` (`gmod_net_unload_all_cl`) to unload them. Modules can be hot-reloaded, so one doesn't need to quit game to see changes.

## License

Whole project is licensed under MIT License.
