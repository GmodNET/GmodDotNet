# Gmod.NET

Cross-platform .NET Module/Plugin platform for Garry's Mod powered by __NET Core__.

## About

Gmod.NET is Garry's Mod Module/Plugin loader for C#
and other .NET languages which runs across all platforms (Windows,
Linux, MacOs). Gmod.NET allows you to develop Garry's Mod extensions without
need to close or reload your game or server.

## Current progress

Latest version was heavily reworked due to the change of the runtime from __Mono__ to __CoreCLR__. Right now Gmod.NET is able to be loaded by Garry's Mod, print welcome message to file, gracefully cleanup unload itself, and to be reloaded by running Garry's Mod.

TODO: Managed wrapper around Garry's mod Lua engine API, Module management system.

## Building and contributing

Gmod.NET is subdivided into two subprojects. Garry's Mod binary native module is
contained in __gm_dotnet_native__ folder. It is written in __C++__ and uses
__CMake__ as its build (prebuild) system.

Managed part is an msbuild solution developed against `netcoreapp3.0` specification.

## Installation

1) Build __gm_dotnet_native__ binaries and place them in `garrysmod/lua/bin` folder.

2) Download latest __NET Core 3.0__ runtime binaries from here https://dotnet.microsoft.com/download/dotnet-core/3.0 and copy `host` and `share` folders to `garrysmod/lua/bin/dotnet`.

3) Build __gm_dotnet_managed__ managed binaries and copy `GmodNET.dll`, `GmodNET.deps.json`, `GmodNET.runtimeconfig.json`, and all dependencies to `garrysmod/lua/bin` folder.

## License

Whole project is licensed under MIT License.
