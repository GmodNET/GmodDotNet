[![Nuget](https://img.shields.io/nuget/v/GmodNET.API?color=blue)](https://www.nuget.org/packages/GmodNET.API/) [![Downloads](https://img.shields.io/nuget/dt/GmodNET.API?color=2db94e)](https://www.nuget.org/packages/GmodNET.API/) [![Discord Server](https://img.shields.io/discord/632622505848471554?label=Our%20Discord&color=2db94e)](https://discord.gg/9bP8nMT)
[![CI](https://github.com/GlebChili/GmodDotNet/workflows/CI/badge.svg?branch=master)](https://github.com/GlebChili/GmodDotNet/actions?query=workflow%3ACI)

# Gmod.NET
[![Current Runtime](https://img.shields.io/badge/Current%20Runtime-0.6.0-2db94e)](https://github.com/GlebChili/GmodDotNet/wiki/GmodNET-Runtime-and-GmodNET.API-version-correspondence#gmodnet-and-gmodnetapi) [![Current API](https://img.shields.io/badge/Current%20API-0.6.0-2db94e)](https://github.com/GlebChili/GmodDotNet/wiki/GmodNET-Runtime-and-GmodNET.API-version-correspondence#gmodnet-and-gmodnetapi)

Cross-platform .NET Module/Plugin platform for Garry's Mod powered by [__.NET Core__](https://dotnet.microsoft.com/).

## About

Gmod.NET is Garry's Mod Module/Plugin loader for C# and other .NET languages which runs across all platforms (Windows, Linux, Mac Os). Gmod.NET allows you to develop cross-platform Garry's Mod extensions without need to close or reload your game or server.

## Current features

GmodNET provides functionality to write Garry's Mod modules in C# or any other CIL-compiled language as [__.NET 5.0__](https://dotnet.microsoft.com/) class libraries. For more information on modules and the API check out [documentation](https://docs.gmodnet.xyz). Only `x86_64` version of Garry's Mod is currently supported.

## Installation and usage

1. Download latest build from the project's [releases page](https://github.com/GlebChili/GmodDotNet/releases).

2. Unpack archive for your OS to the `%GARRYS_MOD_ROOT_FOLDER%garrysmod/lua/bin/` folder.

3. Create a `Modules` folder inside `garrysmod/lua/bin/`.

4. Create and build your .NET Module. For this example we'll call it `ExampleModuleName`. Check out [one of the tutorials on our documentation](https://docs.gmodnet.xyz/articles/tutorials/hello-world/) for examples.

5. Place your .NET module, ...deps.json file, and all dependencies in a newly created `Modules/ExampleModuleName/` folder. *Note that the foldername must be the same as the name of your module's .dll file, without the .dll extension.*

6. If you signed your module with [GmodNetModuleSigner](https://github.com/GlebChili/GmodNetModuleSigner), copy `ExampleModuleName.modulekey` and `ExampleModuleName.modulesign` to the same folder as above (`Modules/ExampleModuleName/`). *Replace `ExampleModuleName` with your module name (without .dll).*

For more detailed installation and usage instructions check out the articles in the [documentation](https://docs.gmodnet.xyz).

## Need help?

Check out our [documentation](https://docs.gmodnet.xyz) or join our [discord server](https://discord.gg/9bP8nMT).

## Contributing and building Gmod.NET

### Build instructions

To build GmodDotNet you need to have following software installed and registered with your PATH environment variable:

1. Latest version of git

2. Latest version of CMake

3. Latest version of dotnet SDK

4. (On Windows) Latest version of Visual Studio 2019 with C++ package

5. (On macOS) Latest version of Xcode

6. (On Linux) Latest versions of make and gcc (or clang)

Build steps:

1. Clone this git repository (building outside of git repository is not supported)

2. In the root of the cloned repository run `dotnet build runtime.csproj -c Debug` or `dotnet build runtime.csproj -c Release` instruction in your command prompt.

__NOTE__: `runtime.csproj` is not a real C# project file but a kind of build script. To work with the managed part of GmodDotNet open `gm_dotnet_managed/gm_dotnet_managed.sln` solution file in your IDE instead.

`runtime.csproj` build script will produce following folders in the root of repository:

1. `build` folder contains the full build of GmodDotNet runtime (both native and managed). The content of this folder should be copied to `garrysmod/lua/bin`.

2. `Modules` folder contains tests suit. To run tests this folder should be copied to `garrysmod/lua/bin`.

3. `nupkgs` folder contains `GmodNET.API` NuGet package.

You may also want to copy the content of `lua` folder to the corresponding destinations in `garrysmod/lua`.

### Folder structure

Gmod.NET is subdivided into three subprojects:

* `gm_dotnet_native`: Garry's Mod binary native module and helper libraries organized as a CMake project. *Loads the main Gmod.NET module.*
* `gm_dotnet_managed`: Managed projects organized using the .NET soultion file `gm_dotnet_managed.sln`. *Loads and unloads modules, exposes (Lua) functions and capabilities to those modules.*
* `lua`: Lua scripts, currently only for loading the test runner module.

### Nightly builds

You can find latest nightly builds GmodDotNet runtime at http://nightly.gmodnet.xyz/. To use nightly NuGet packages connect to [our nightly NuGet feed](https://dev.azure.com/GmodNET/gmodnet-artifacts/_packaging?_a=feed&feed=gmodnet-packages).

[This article in the documentation](https://docs.gmodnet.xyz/articles/tutorials/connect-nightly/) describes how to use these nightly builds.

## Similar projects

Check out [gmod_csModuleLoader](https://github.com/dedady157/gmod_csModuleLoader) by [Bailey Drahoss](https://github.com/dedady157).

## License

Whole project is licensed under MIT License.

## Dependencies and code usage

Gmod.NET is making use of or borrows code from the following projects:

1. [CoreCLR](https://github.com/dotnet/coreclr), [CoreFX](https://github.com/dotnet/corefx), and [core-setup](https://github.com/dotnet/core-setup) by [.NET Foundation](https://github.com/dotnet) (MIT License)

2. [pure_lua_SHA](https://github.com/Egor-Skriptunoff/pure_lua_SHA) by [Egor Skriptunoff](https://github.com/Egor-Skriptunoff) (MIT License)

3. [NSec](https://nsec.rocks/) by [Klaus Hartke](https://github.com/ektrah) (MIT License)

4. [Libsodium](http://libsodium.org) by [Frank Denis](https://github.com/jedisct1) (ISC License)

See other copyright notices in the NOTICE file.
