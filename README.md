# Gmod.NET
Integration of Mono into Garry's Mod as in-game scripting backend.

## Garry's mod version tested
Currently only Windows 64-bit builds of Garry's mod tested. To learn more about Garry's mod 64-bit builds read this:
https://forum.facepunch.com/f/gmoddev/btgwr/64-bit-Garry-s-Mod/1/

## Linux and MacOs support
Currently whole project consists only of Visual Studio 2017 solutions. Support for Linux(both client and server) and MacOs will definitely come, but later.

## Current features
Ability to make basic Lua engine calls (LUA->Push, LUA->GetField, etc) from managed code with GmodNET.Lua.Api singleton.

## Features to implement next
Expose remaining Lua engine API to GmodNET.Lua class. Expose all game specific classes (QAngle, Entity, etc) to the managed code.

## Build instructions
Coming soon.

## License
MIT
