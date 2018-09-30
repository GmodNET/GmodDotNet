#include <string>

#include <GarrysMod/Lua/Interface.h>

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>

using namespace std;
using namespace GarrysMod;
using namespace GarrysMod::Lua;

//Change this variabe for building for another os: "\\" for windows and "/" for linux/macos
const string Path_Separator = "\\";
const char Path_Separator_Char = Path_Separator[0];

//Variable to store pointer on Lua api. Must be initiated in GNOD_MODULE_OPEN.
ILuaBase* Glua;

//Variables to store mono runtime data
MonoDomain* mainDomain = NULL;
MonoAssembly* mainAssembly = NULL;
MonoImage* mainImage = NULL;

//Function to print message to the console
void GmodPrint(string msg)
{
	//Push special blob on stack
	Glua->PushSpecial(0);
	//Push print function on stack
	Glua->GetField(-1, "print");
	//Push message to print on stack
	Glua->PushString(msg.c_str());
	//Make a call from the stack
	Glua->Call(1, 0);
	//Clean stack
	Glua->Pop();
}

//Function to return full path to the directory
string GmodModuleDirectory()
{
	//Lua api calls
	Glua->PushSpecial(SPECIAL_GLOB);
	Glua->GetField(-1, "util");
	Glua->GetField(-1, "RelativePathToFull");
	Glua->PushString("gameinfo.txt");
	Glua->Call(1, 1);
	const char* tmp_path;
	tmp_path = Glua->GetString();
	//Get path
	string path(tmp_path);
	//Clear stack
	Glua->Pop(3);
	//Resize path
	for (int i = path.length() - 1; i >= 0; i--)
	{
		if (path[i] == Path_Separator_Char)
		{
			path.resize(i);
			break;
		}
	}
	path += Path_Separator + "lua" + Path_Separator +"bin";

	return path;
}


//Make wrappers for lua api to pass to mananage world
void LuaPushSpecial(int i) //
{
	Glua->PushSpecial(i);
}
void LuaPush(int i) //
{
	Glua->Push(i);
}
void LuaPushString(MonoString* str) //
{
	char* tmp = mono_string_to_utf8(str);
	Glua->PushString(tmp);
	mono_free(tmp);
}
void LuaPushNumber(double d) //
{
	Glua->PushNumber(d);
}
void LuaPushBool(MonoObject* boolean) //
{
	Glua->PushBool(*(bool*)mono_object_unbox(boolean));
}
void LuaGetField(int stackPos, MonoString* name) //
{
	char* tmp = mono_string_to_utf8(name);
	Glua->GetField(stackPos, tmp);
	mono_free(tmp);
}
void LuaCall(int num_of_args, int num_of_returnes) //
{
	Glua->Call(num_of_args, num_of_returnes);
}
void LuaPop(int i) //
{
	Glua->Pop(i);
}
double LuaGetNumber(int i) //
{
	return Glua->GetNumber(i);
}
MonoString* LuaGetString(int i) //
{
	const char* tmp_str = Glua->GetString(i);
	MonoString* str = mono_string_new(mainDomain, tmp_str);
	return str;
}
MonoObject* LuaGetBool(int i) //
{
	bool tmp_bool = Glua->GetBool(i);
	//Get boolen class
	MonoClass* bl_class = mono_get_boolean_class();
	//Box tmp_bool into managed world
	MonoObject* rt = mono_value_box(mainDomain, bl_class, &tmp_bool);
	return rt;
}

//Method to register functions with Mono runtime
void RegisterFunctions()
{
	mono_add_internal_call("GmodNET.Lua::IntPushSpecial(int)", LuaPushSpecial);
	mono_add_internal_call("GmodNET.Lua::IntPush(int)", LuaPush);
	mono_add_internal_call("GmodNET.Lua::IntPushString(string)", LuaPushString);
	mono_add_internal_call("GmodNET.Lua::IntPushNumber(double)", LuaPushNumber);
	mono_add_internal_call("GmodNET.Lua::IntPushBool(bool)", LuaPushBool);
	mono_add_internal_call("GmodNET.Lua::IntGetField(int,string)", LuaGetField);
	mono_add_internal_call("GmodNET.Lua::IntCall(int,int)", LuaCall);
	mono_add_internal_call("GmodNET.Lua::IntPop(int)", LuaPop);
	mono_add_internal_call("GmodNET.Lua::IntGetNumber(int)", LuaGetNumber);
	mono_add_internal_call("GmodNET.Lua::IntGetString(int)", LuaGetString);
	mono_add_internal_call("GmodNET.Lua::IntGetBool(int)", LuaGetBool);
}

//Function to setup assembly
void SetUpAssembly()
{
	//Open assembly
	GmodPrint("Loading main assembly...");
	mainAssembly = mono_domain_assembly_open(mainDomain, (GmodModuleDirectory() + Path_Separator + "Gmod.NET.dll").c_str());
	if (mainAssembly)
	{
		GmodPrint("Main assembly was successfully loaded!");
	}
	else
	{
		GmodPrint("ERROR: Unable to load main assembly!");
		return;
	}
	//Get image of the assembly
	GmodPrint("Getting image of the assembly...");
	mainImage = mono_assembly_get_image(mainAssembly);
	if (mainImage)
	{
		GmodPrint("Image was successfully loaded!");
	}
	else
	{
		GmodPrint("ERROR: Unable to load image!");
		return;
	}
}

//Function called on module load
GMOD_MODULE_OPEN()
{
	//Initing GLua
	Glua = LUA;
	//Print msg
	GmodPrint("Loading Gmod.NET Native Module...");
	//Set up mono folders
	GmodPrint("Setting up mono directories...");
	mono_set_dirs((GmodModuleDirectory() + Path_Separator + "lib").c_str(), (GmodModuleDirectory() + Path_Separator + "etc").c_str());
	
	//Init mono domain
	GmodPrint("Creating mono domain...");
	mainDomain = mono_jit_init("mainDomain");
	if (mainDomain)
	{
		GmodPrint("Mono domain was successfully created!");
	}
	else
	{
		GmodPrint("ERROR: Unable to create mono domain!");
		return 0;
	}
	SetUpAssembly();
	//Print message on load
	if (mainDomain && mainAssembly && mainImage)
	{
		GmodPrint("Mono is running!");
	}
	else
	{
		GmodPrint("Unable to set up Mono!");
		return 0;
	}
	//Register internal calls
	GmodPrint("Registering internal calls...");
	RegisterFunctions();
	//Pass control to the managed code
	GmodPrint("Passing control to the managed code...");
	//Get class with main method
	MonoClass* mainClass = mono_class_from_name(mainImage, "GmodNET", "GmodNET");
	//Check
	if (!mainClass)
	{
		GmodPrint("ERROR: Unable to find method with entry point!");
		return 0;
	}
	//Create method description
	MonoMethodDesc* MainMethodDesc = mono_method_desc_new("GmodNET.GmodNET::Main()", true);
	//Get method pointer
	MonoMethod* MainMethod = mono_method_desc_search_in_class(MainMethodDesc, mainClass);
	//Check
	if (!MainMethod)
	{
		GmodPrint("ERROR: Unable to find entry point!");
		return 0;
	}
	//Invoke main
	mono_runtime_invoke(MainMethod, NULL, NULL, NULL);
	return 0;
}


//Function called on module close
GMOD_MODULE_CLOSE()
{
	//Free mono domain
	mono_jit_cleanup(mainDomain);

	return 0;
}