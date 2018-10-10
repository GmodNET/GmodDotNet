# Gmod.NET
Integration of Mono into Garry's Mod as in-game scripting backend.

## Garry's mod version tested
Tested with:

[1] Windows 10: x32 and x64 builds of the game

[2] Ubuntu 16.04: x32 server build. Current x64 server build of the Garry's Mod is not functional and therefore wasn't tested.

[3] MacOs: I don't have an access to any Apple computer now and thus can't test anything. If you are ready to help with this feel free to write me: gleb@krasilich.net

To learn more about Garry's mod 64-bit builds read this:
https://forum.facepunch.com/f/gmoddev/btgwr/64-bit-Garry-s-Mod/1/

## Linux and MacOs support
Currently whole project consists only of Visual Studio 2017 solutions. Linux binaries are built directly with g++ right now. Support for Makefile will come latter. Build options for MacOs are under research.

## Current features
Ability to make basic Lua engine calls (LUA->Push, LUA->GetField, etc) from managed code with GmodNET.Lua.Api singleton. Ability to register managed
functions with Garry's mod Lua engine (can be tested with lua_run testFunc() from the game).

To test mode run following: lua_run require("gmodnet"). This will load binary module.

## Features to implement next
Expose remaining Lua engine API to GmodNET.Lua class. Expose all game specific classes (QAngle, Entity, etc) to the managed code.

## Build instructions
See BuildInst.txt

## License
MIT
