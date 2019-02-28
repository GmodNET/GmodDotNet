/*
 Authors:
	Gleb Krasilich(gleb@krasilich.net)
 2019
*/

#include "GarrysMod/Lua/Interface.h"
#include <string>

//Print message to console. First argument: Garry's mod lua engine stack pointer. Second argument: message to print
void gprint(GarrysMod::Lua::ILuaBase* lua, std::string message);