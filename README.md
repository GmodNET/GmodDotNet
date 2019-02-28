# Gmod.NET

Cross-platform .NET Module/Plugin platform for Garry's Mod powered by __Mono__.

## Versions

Check GitHub tags for getting "stable" versions. NOTE: Major changes (including project conception) occurred
since version 0.1.

## About

Gmod.NET is intended to be Garry's Mod Module/Plugin loader for C#
and other .NET languages which run across all platforms (Windows,
Linux, MacOs). Gmod.NET allows you to develop Garry's Mod extensions
with one of the best programming languages available on market and without
need to close or reload your game or server.

Gmod.NET provides full access C# API for Garry's Mod Lua Engine
so you are able to do everything possible with Lua or C++ modules.
You also have an access to latest .NET Framework API with Mono extensions.

As part of the project I am also planning to write a Gmod.NET module
which will provide Wiremod Expression 2 like in-game scripting with
C#.

## Architecture

Gmod.NET Modules are just classes which implement `Gmod.NET.IModule`
interface from Gmod.NET.dll assembly. Modules are instantiated with
default (argumentless) constructors and called CleanUp upon unloading.
Modules are responsible for deleting all Lua references used or created
by one.

Modules can interact with Garry's Mod by calling `Gmod.NET.LuaStack` and
`Gmod.NET.Lua.Engine` methods.

## Building and contributing

Gmod.NET is subdivided into two subprojects. Garry's Mod binary Lua extension is
contained in __gm_dotnet_native__ folder. It is written in __C++ 17__ and uses
__CMake__ as its build (prebuild) system.

Managed part, which is shared .NET Framework class library, is contained in
__gm_dotnet_managed__ folder. It is written in __C# 7.3__ and organized as
__Micrsoft Visual Studio Solution__. It can be opened with __Microsoft Visual Studio__
on Mac and Windows, __MonoDevelop__ and __JetBrains Rider__ on Linux, or simply built with
__MSBuild__ (__Mono__ version of __MSBuild__ is required to build the solution on non-Windows platforms,
you can't build it with __.NET Core__ distributed version of __MSBuild__).

__gm_dotnet_managed__ may use some __Mono__ specific .NET Framework extensions like `System.MathF` class.
Keep it mind when dealing with dependencies.

## License

Whole project is licensed under MIT License.
