# dotnet
The Lua library to interact with the GmodDotNet runtime.
This library is added to the Lua global scope after GmodDotNet runtime is loaded to the game with
`require("dotnet")` (see [require](https://wiki.facepunch.com/gmod/Global.require) for more info).

## Functions

### load(string)
Loads a GmodDotNet module by its name.

##### Declaration
```lua
boolean dotnet.load(string module_name)
```

##### Parameters
| Type | Name | Description |
|------|------|-------------|
| [string](https://wiki.facepunch.com/gmod/string) | module_name | A name of the module or module's absolute path |

##### Returns
| Type | Description |
|------|-------------|
| [boolean](https://wiki.facepunch.com/gmod/boolean) | `True`, if the module was loaded successfully, `False` otherwise.|

##### Remarks
Loads a GmodDotNet module at path `[garrys mod (or dedicated server) root folder]/garrysmod/lua/bin/Modules/[module name]/[module name].dll`.
If [Development environment](xref:runtime_features_development_environment) is on, `module_name` can be an absolute path to module's dll and GmodDotNet runtime will load it.

Module must be accompanied by all its dependencies and `[module name].deps.json` file.
In practice it means that folder `[garrys mod root folder]/garrysmod/lua/bin/Modules/[module name]` must contain the full output of [`dotnet publish`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) operation of dotnet sdk.

If GmodDotNet runtime is not able to load the module, the function will return `False`, and the error message will be printed to a game console.

### unload(string)
Unloads a GmodDotNet module by its name.

##### Declaration
```lua
boolean dotnet.unload(string module_name)
```

##### Parameters
| Type | Name | Description |
|------|------|-------------|
| [string](https://wiki.facepunch.com/gmod/string) | module_name | A name of the module to unload |

##### Returns
| Type | Description |
|------|-------------|
| [boolean](https://wiki.facepunch.com/gmod/boolean) | `True`, if the module was unloaded successfully, `False` otherwise.|

##### Remarks
Parameter `module_name` must be a name of the module by which it was loaded. 
For example, if module was loaded by an absolute path in [Development environment](xref:runtime_features_development_environment) it also should be unloaded by its absolute path.

If GmodDotNet runtime is not able to unload the module (exception was thrown while unloading, module is not able to free its resources, module with given name is not loaded, etc.), the function will return `False`, and the error message will be printed to a game console.
