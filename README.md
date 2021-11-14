[![Nuget](https://img.shields.io/nuget/v/GmodNET.API?color=blue)](https://www.nuget.org/packages/GmodNET.API/) [![Downloads](https://img.shields.io/nuget/dt/GmodNET.API?color=2db94e)](https://www.nuget.org/packages/GmodNET.API/) [![Discord Server](https://img.shields.io/discord/632622505848471554?label=Our%20Discord&color=2db94e)](https://discord.gg/9bP8nMT)
[![CI](https://github.com/GlebChili/GmodDotNet/workflows/CI/badge.svg?branch=master)](https://github.com/GlebChili/GmodDotNet/actions?query=workflow%3ACI)

# Gmod.NET

Cross-platform .NET Module/Plugin platform for Garry's Mod powered by [__.NET__](https://dotnet.microsoft.com/).

## About

Gmod.NET is Garry's Mod Module/Plugin loader for C#
and other .NET languages which runs across all platforms (Windows,
Linux, Mac Os). Gmod.NET allows you to develop cross-platform Garry's Mod extensions without
need to close or reload your game or server.

Gmod.NET allows you to write Garry's Mod modules in C# or any other CIL-compiled language as [__.NET 6.0__](https://dotnet.microsoft.com/) class libraries. 
For more information on modules and API check out [our documentation](https://docs.gmodnet.xyz/). 
Only `x86_64` version of Garry's Mod is currently supported.

## Need help?

Check out [our docs](https://docs.gmodnet.xyz/) and join our [discord server](https://discord.gg/9bP8nMT).

## Building and contributing

### Build instructions

To build GmodDotNet you need to have following software installed and registered with your PATH environment variable:

1. Latest version of git

2. Latest version of CMake

3. Latest version of dotnet SDK

4. (On Windows) Latest version of Visual Studio 2019 with C++ workload

5. (On macOS) Latest version of Xcode

6. (On Linux) Latest versions of make and gcc (or clang)

Build steps:

1. Clone this git repository (building outside of git repository is not supported)

2. In the root of the cloned repository run `dotnet build runtime.csproj -c Debug` or `dotnet build runtime.csproj -c Release` instruction in your command prompt.

__NOTE__: `runtime.csproj` is not a real C# project file but a kind of build script. To work with the managed part of Gmod.NET open `gm_dotnet_managed/gm_dotnet_managed.sln` solution file in your IDE instead.

`runtime.csproj` build script will produce following folders in the root of repository:

1. `build` folder contains the full build of GmodDotNet runtime (both native and managed). The content of this folder should be copied to `garrysmod/lua/bin`.

2. `Modules` folder contains tests suit. To run tests this folder should be copied to `garrysmod/lua/bin`.

3. `nupkgs` folder contains `GmodNET.API` NuGet package.

### Nightly builds

You can find latest nightly builds of Gmod.NET runtime at http://nightly.gmodnet.xyz/. To use our nightly NuGet packages connect to [our nightly NuGet feed](https://dev.azure.com/GmodNET/gmodnet-artifacts/_packaging?_a=feed&feed=gmodnet-packages).

## Installation and usage

1. Download latest build from the project's [releases page](https://github.com/GlebChili/GmodDotNet/releases).

2. Unpack archive for your OS to the `%GARRYS_MOD_ROOT_FOLDER%garrysmod/lua/bin/` folder.

3. Create a `Modules` folder inside `garrysmod/lua/bin/`.

4. Place your .NET module, ...deps.json file, and all dependencies in `Modules/%exact_name_of_the_module_without_dll%/` folder.

5. Load Gmod.NET runtime in game by executing Lua function `require("dotnet")`.

6. Load your module by running Lua function `dotnet.load("%exact_name_of_the_module_without_dll%")`.

For more info check out [our quick start guide](https://docs.gmodnet.xyz/articles/tutorials/hello-world/index.html).

## License

Whole project is licensed under MIT License.

## Dependencies and code usage

See the NOTICE file.
