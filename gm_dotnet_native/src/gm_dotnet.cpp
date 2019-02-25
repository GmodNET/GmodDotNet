#include "GarrysMod/Lua/Interface.h"
#include "gprint.h"
#include <string>

#define VER_MAJOR "0"
#define VER_MINOR "2"
#define VER_MISC "0"

using namespace std;

//Global LUA state
GarrysMod::Lua::ILuaBase * Global_lua;

GMOD_MODULE_OPEN()
{
	//Print welcome message
	gprint(LUA, "gm_dotnet_native loaded! Version " + string(VER_MAJOR) + "." + string(VER_MINOR) + "." + string(VER_MISC));
	//Export global lua
	Global_lua = LUA;
	//Display Mono information
	
	return 0;
}

GMOD_MODULE_CLOSE()
{
	gprint(LUA, "gm_dontet_native closed!");

	return 0;
}