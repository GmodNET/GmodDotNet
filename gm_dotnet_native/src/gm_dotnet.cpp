/*
 Authors:
	Gleb Krasilich (gleb@krasilich.net)
 2019
*/

#include "GarrysMod/Lua/Interface.h"
#include "gprint.h"
#include <string>
#include "version.h"
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>


// Global LUA state pointer
GarrysMod::Lua::ILuaBase * Global_lua = NULL;

// Global Mono domain
MonoDomain * MainDomain = NULL;

// Global main assembly
MonoAssembly * MainAssembly = NULL;

// Global main image
MonoImage * MainImage = NULL;

// Bridges to managed world

// Push special bridge
void BridgePushSpecial(int index)
{
	Global_lua->PushSpecial(index);
}
// Push integer bridge
void BridgePushNumber(double number)
{
	Global_lua->PushNumber(number);
}
// Push string bridge
void BridgePushString(MonoString * str)
{
	char * ustr = mono_string_to_utf8(str);
	Global_lua->PushString(ustr);
	mono_free(ustr);
}
// Push bool bridge
void BridgePushBool(char b)
{
	if (b == 0x0)
	{
		Global_lua->PushBool(false);
	}
	else
	{
		Global_lua->PushBool(true);
	}
}
// Get Field bridge
void BridgeGetField(int pos, MonoString * key)
{
	char * ustr = mono_string_to_utf8(key);
	Global_lua->GetField(pos, ustr);
	mono_free(ustr);
}
// Bridge Set Field
void BridgeSetField(int pos, MonoString * key)
{
	char * ustr = mono_string_to_utf8(key);
	Global_lua->SetField(pos, ustr);
	mono_free(ustr);
}
// Bridge Push Function
void BridgePushFunction(GarrysMod::Lua::CFunc c)
{
	Global_lua->PushCFunction(c);
}
// Bridge Pop
void BridgePop(int num)
{
	Global_lua->Pop(num);
}
// Bridge Push
void BridgePush(int pos)
{
	Global_lua->Push(pos);
}
// Bridge Call
void BridgeCall(int num_of_arg, int num_of_returns)
{
	Global_lua->Call(num_of_arg, num_of_returns);
}
// Bridge PCall
int BridgePCall(int num_of_args, int num_of_returns, int error_func)
{
	return Global_lua->PCall(num_of_args, num_of_returns, error_func);
}
// Bridge Get Number
double BridgeGetNumber(int pos)
{
	return Global_lua->GetNumber(pos);
}
// Bridge Get Bool
char BridgeGetBool(int pos)
{
	bool tmp = Global_lua->GetBool(pos);
	if (!tmp)
	{
		return 0x0;
	}
	else
	{
		return 0x1;
	}
}
// Bridge Get String
struct get_string_struct
{
	int length;
	const char * ptr;
};
const get_string_struct BridgeGetString(int pos)
{
	const char * tmp = Global_lua->GetString(pos);
	if (tmp == NULL)
	{
		return { 0, NULL };
	}
	else
	{
		int t_l = strlen(tmp);
		return { t_l, tmp };
	}
}

// Bridge Get Function
GarrysMod::Lua::CFunc BridgeGetFunction(int pos)
{
	return Global_lua->GetCFunction(pos);
}

// Bridge Push Nil
void BridgePushNil()
{
	Global_lua->PushNil();
}

// Bridge Push Vector
void BridgePushVector(float x, float y, float z)
{
	Vector tmp;
	tmp.x = x;
	tmp.y = y;
	tmp.z = z;
	Global_lua->PushVector(tmp);
}

// Bridge Push Angle
void BridgePushAngle(float pitch, float yaw, float roll)
{
	QAngle tmp;
	tmp.x = pitch;
	tmp.y = yaw;
	tmp.z = roll;
	Global_lua->PushAngle(tmp);
}

// Bridge Get Vector
struct get_vector_struct
{
	float x;
	float y;
	float z;
};
get_vector_struct BridgeGetVector(int pos)
{
	Vector tmp = Global_lua->GetVector(pos);
	get_vector_struct ret = { tmp.x, tmp.y, tmp.z };
	return ret;
}

// Bridge Get Angle
get_vector_struct BridgeGetAngle(int pos)
{
	QAngle tmp = Global_lua->GetAngle(pos);

	get_vector_struct ret = { tmp.x, tmp.y, tmp.z };

	return ret;
}

// Bridge Create Table
void BridgeCreateTable()
{
	Global_lua->CreateTable();
}

// Bridge Create Meta Table
int BridgeCreateMetaTable(MonoString * name)
{
	char * ustr = mono_string_to_utf8(name);
	int ret = Global_lua->CreateMetaTable(ustr);
	mono_free(ustr);
	return ret;
}

// Bridge Get Type
int BridgeGetType(int pos)
{
	return Global_lua->GetType(pos);
}

// Bridge Get Type Name
get_string_struct BridgeGetTypeName(int type_id)
{
	const char * name = Global_lua->GetTypeName(type_id);
	if (name == NULL)
	{
		return { 0, NULL };
	}
	else
	{
		int len = strlen(name);
		return { len, name };
	}
}

// Bridge Get Meta Table
char BridgeGetMetaTable(int pos)
{
	bool tmp = Global_lua->GetMetaTable(pos);
	if (!tmp)
	{
		return 0x0;
	}
	else
	{
		return 0x1;
	}
}

// Bridge Push Meta Table
char BridgePushMetaTable(int type)
{
	bool tmp = Global_lua->PushMetaTable(type);
	if (!tmp)
	{
		return 0x0;
	}
	else
	{
		return 0x1;
	}
}

//Method called by Garry's mod on module load
GMOD_MODULE_OPEN()
{
	// Print welcome message
	gprint(LUA, "gm_dotnet_native was loaded! Version " + std::string(VER_MAJOR) + "." + std::string(VER_MINOR) + "." + 
		std::string(VER_MISC) + " " + std::string(VER_NAME) + ".");
	//Print Mono runtime information
	{
		char * rnt_info = mono_get_runtime_build_info();
		std::string display_info = "Mono Runtime used: " + std::string(rnt_info);
		mono_free(rnt_info);
		gprint(LUA, display_info);
	}
	// Export global lua
	Global_lua = LUA;
	// Pass runtime flags to mono
	char * mono_runtime_options[] = { "-O=float32" };
	mono_jit_parse_options(1, (char**)mono_runtime_options);
	// Set mono directories
	mono_set_dirs("garrysmod/lua/bin/gm_dotnet", "garrysmod/lua/bin/gm_dotnet_conf");
	//Init Mono Domain
	MainDomain = mono_jit_init("Gmod.NET domain");
	if (!MainDomain)
	{
		gprint(LUA, "Unable to create Mono domain! Loading process was stopped!");

		return 0;
	}

	//Register internal calls
	mono_add_internal_call("Gmod.NET.LuaStack::push_special(int)", (void*)BridgePushSpecial);
	mono_add_internal_call("Gmod.NET.LuaStack::push_number(double)", (void*)BridgePushNumber);
	mono_add_internal_call("Gmod.NET.LuaStack::push_string(string)", (void*)BridgePushString);
	mono_add_internal_call("Gmod.NET.LuaStack::push_bool(byte)", (void*)BridgePushBool);
	mono_add_internal_call("Gmod.NET.LuaStack::get_field(int,string)", (void*)BridgeGetField);
	mono_add_internal_call("Gmod.NET.LuaStack::set_field(int,string)", (void*)BridgeSetField);
	mono_add_internal_call("Gmod.NET.LuaStack::push_function(intptr)", (void*)BridgePushFunction);
	mono_add_internal_call("Gmod.NET.LuaStack::pop(int)", (void*)BridgePop);
	mono_add_internal_call("Gmod.NET.LuaStack::push(int)", (void*)BridgePush);
	mono_add_internal_call("Gmod.NET.LuaStack::call(int,int)", (void*)BridgeCall);
	mono_add_internal_call("Gmod.NET.LuaStack::pcall(int,int,int)", (void*)BridgePCall);
	mono_add_internal_call("Gmod.NET.LuaStack::get_number(int)", (void*)BridgeGetNumber);
	mono_add_internal_call("Gmod.NET.LuaStack::get_bool(int)", (void*)BridgeGetBool);
	mono_add_internal_call("Gmod.NET.LuaStack::get_string(int)", (void*)BridgeGetString);
	mono_add_internal_call("Gmod.NET.LuaStack::get_function(int)", (void*)BridgeGetFunction);
	mono_add_internal_call("Gmod.NET.LuaStack::push_nil()", (void*)BridgePushNil);
	mono_add_internal_call("Gmod.NET.LuaStack::push_vector(single,single,single)", (void*)BridgePushVector);
	mono_add_internal_call("Gmod.NET.LuaStack::push_angle(single,single,single)", (void*)BridgePushAngle);
	mono_add_internal_call("Gmod.NET.LuaStack::get_vector(int)", (void*)BridgeGetVector);
	mono_add_internal_call("Gmod.NET.LuaStack::get_angle(int)", (void*)BridgeGetAngle);
	mono_add_internal_call("Gmod.NET.LuaStack::create_table()", (void*)BridgeCreateTable);
	mono_add_internal_call("Gmod.NET.LuaStack::create_meta_table(string)", (void*)BridgeCreateMetaTable);
	mono_add_internal_call("Gmod.NET.LuaStack::get_type(int)", (void*)BridgeGetType);
	mono_add_internal_call("Gmod.NET.LuaStack::get_type_name(int)", (void*)BridgeGetTypeName);
	mono_add_internal_call("Gmod.NET.LuaStack::get_meta_table(int)", (void*)BridgeGetMetaTable);
	mono_add_internal_call("Gmod.NET.LuaStack::push_meta_table(int)", (void*)BridgePushMetaTable);

	//Load main assembly
	MainAssembly = mono_domain_assembly_open(MainDomain, "garrysmod/lua/bin/gm_dotnet/Gmod.NET.dll");
	if (!MainAssembly)
	{
		gprint(LUA, "Unable to load Gmod.NET.dll! Loading process was stopped!");

		return 0;
	}

	//Get Image of the main assembly
	MainImage = mono_assembly_get_image(MainAssembly);
	if (!MainImage)
	{
		gprint(LUA, "Unable to load image of main assembly! Loading process was stopped!");

		return 0;
	}

	//Init main method descriptor
	MonoMethodDesc * main_method_desc = mono_method_desc_new("Gmod.NET.Gmod::Main()", true);

	//Get main method
	MonoMethod * main_method = mono_method_desc_search_in_image(main_method_desc, MainImage);

	//Free memory
	mono_method_desc_free(main_method_desc);

	if (!main_method)
	{
		gprint(LUA, "Unable to find entry point! Loading process was stopped!");

		return 0;
	}

	//Pass controll to Gmod.NET
	mono_runtime_invoke(main_method, NULL, NULL, NULL);

	return 0;
}

//Method called on module closed
GMOD_MODULE_CLOSE()
{
	if (MainDomain)
	{
		mono_jit_cleanup(MainDomain);
		MainDomain = NULL;
		MainAssembly = NULL;
	}
	//Print message on module unload
	gprint(LUA, "gm_dotnet_native was unloaded!");

	return 0;
}