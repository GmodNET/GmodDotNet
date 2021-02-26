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
| [string](https://wiki.facepunch.com/gmod/string) | module_name | A name of the module to load |

##### Returns
| Type | Description |
|------|-------------|
| [boolean](https://wiki.facepunch.com/gmod/boolean) | `True`, if the module was loaded successfully, `False` otherwise.|

##### Remarks
Loads a GmodDotNet module at path `[garrys mod (or dedicated server) root folder]/garrysmod/lua/bin/Modules/[module name]/[module name].dll`.

Module must be accompanied by all its dependencies and `[module name].deps.json` file.
In practice it means that folder `[garrys mod root folder]/garrysmod/lua/bin/Modules/[module name]` must contain the full output of [`dotnet publish`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) operation of dotnet sdk.

If GmodDotNet runtime is not able to load the module, the function will return `False`, and the error message will be printed to a game console.
