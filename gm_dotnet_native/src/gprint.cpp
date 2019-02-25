#include "gprint.h"
#include "GarrysMod/Lua/Interface.h"
#include <string>

using namespace std;

void gprint(GarrysMod::Lua::ILuaBase* lua_stack, string msg)
{
	lua_stack->PushSpecial(0);
	lua_stack->GetField(-1, "print");
	lua_stack->PushString(msg.c_str());
	lua_stack->Call(1, 0);
	lua_stack->Pop(1);
}