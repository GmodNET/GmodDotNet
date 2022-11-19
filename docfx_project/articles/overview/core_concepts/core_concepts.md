---
uid: core_concepts_overview
title: "Core Concepts"
---

# Core Concepts

Gmod.NET modules are essentially .NET (currently version 7.0) class libraries which reference [GmodNET.API nuget package](https://www.nuget.org/packages/GmodNET.API/) as their dependency.
Each module should contain one or more implementation of [GmodNET.API.IModule](xref:GmodNET.API.IModule) interface.
Gmod.NET runtime will instantiate each class derived from `IModule` by calling its parameterless constructor.

## Module lifecycle

At some moment Gmod.NET runtime will call each module's [Load](xref:GmodNET.API.IModule#GmodNET_API_IModule_Load_GmodNET_API_ILua_System_Boolean_GmodNET_API_ModuleAssemblyLoadContext_) method.
```csharp
public void Load(ILua lua, bool is_serverside, ModuleAssemblyLoadContext assembly_context)
```
This method can be thought as an entry point of the module where all initial setup is performed.
The `lua` parameter will receive an instance of [GmodNET.API.ILua](xref:GmodNET.API.ILua) interface which should be used to communicate with Garry's Mod.
The `is_serverside` parameter can be used to easily check whether module is running in server or client environment.
The `assembly_context` parameter receives an instance of [GmodNET.API.ModuleAssemblyLoadContext](xref:GmodNET.API.ModuleAssemblyLoadContext) in which module was loaded,
which can be used for some advanced scenarios or debugging.

All modules also must implement [Unload](xref:GmodNET.API.IModule#GmodNET_API_IModule_Unload_GmodNET_API_ILua_) method.
```csharp
public void Unload(ILua lua);
```
This method is called by Gmod.NET runtime whenever module is unloaded manually or game/server session is over.
The `lua` parameter receives an instance of [GmodNET.API.ILua](xref:GmodNET.API.ILua) interface which can be used to safely perform clenup tasks,
like unregistering callbacks, removing data from global tables, etc.

In order to load Gmod.NET module one should load Gmod.NET runtime via Lua using `require` function:
```lua
require("dotnet")
```
Then module can be loaded via Lua [dotnet.load](xref:lua_api_dotnet#loadstring) function.
Module also can be unloaded with Lua [dotnet.unload](xref:lua_api_dotnet#unloadstring) function.
Generally, modules can be loaded and unloaded at any moment, thus enabling hot-reloading and hot-switching for faster development.

## Module file-structure and dependencies

Modules can bring any .NET or native dependencies (usually acquired as NuGet packages).

Modules' binaries and their placement must satisfy the following convention: module's main dll, its `%modules_name%.deps.json`,
and all dependencies must be placed in `Garrys_mod_root_folder/garrysmod/lua/bin/Modules/%exact_name_of_your_module_without_dll%/` folder.
For example, if your module is called `TestNETModule.dll` and it depends on `SomeLibrary.dll`,
then `TestNETModules.dll`, `TestNETModule.deps.json`, `GmodNET.API.dll` and `SomeLibrary.dll` must be in `garrysmod/lua/bin/Modules/TestNETModule/` folder.
Missing dependencies, `deps.json` files, or wrong folder naming may lead to inability of Gmod.NET runtime to load module.

Gmod.NET runtime will locate and load module's managed and native dependencies into isolated `AssemblyLoadContxt` by reading `*.deps.json` file and using [standard .NET dependency resolution process](https://github.com/dotnet/runtime/blob/main/docs/design/features/host-probing.md).

If Development environment is enabled, Gmod.NET runtime can load modules from any location, not only `garrysmod/lua/bin/Modules` folder.
For more info see [Development environment feature](xref:runtime_features_development_environment).

## Module permissions and safety

Under the hood Gmod.NET uses standard Garry's Mod C++ API.
It means that all Gmod.NET modules have the same permissions and privileges as host Garry's Mod process, including filesystem access, networking, etc.
Thus, ***never run untrusted Gmod.NET modules or start Garry's Mod as an administrator or root***.
