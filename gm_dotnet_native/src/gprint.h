/*
 Authors:
	Gleb Krasilich(gleb@krasilich.net)
 2019
*/

#pragma once
#include "GarrysMod/Lua/Interface.h"
#include <string>

//Print message to console. First argument: lua stack pointer. Second argument: message to print
void gprint(GarrysMod::Lua::ILuaBase* lua_stack, std::string msg);