/* This is a native module of the GmodDotNet. It is linked with mono, inits mono domain, load assembly and calls entry point of
* the managed library.
* 
* Authors: Gleb Krasilich

2018*/


#include <string>

#include <GarrysMod/Lua/Interface.h>

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>

using namespace std;
using namespace GarrysMod;
using namespace GarrysMod::Lua;

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
		if (path[i] == '/' || path[i] == '\\')
		{
			path.resize(i);
			break;
		}
	}
	path += "/lua/bin";

	return path;
}


//Make wrappers for lua api to pass to mananaged world
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
void LuaPushBool(int pre_bool) //
{
	if (pre_bool == 1)
	{
		Glua->PushBool(true);
	}
	else
	{
		Glua->PushBool(false);
	}
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
const char* LuaGetString(int i) //
{
	return Glua->GetString(i);
}
int LuaGetBool(int i) //
{
	bool tmp_bool = Glua->GetBool(i);
	if (tmp_bool)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}
int LuaPCall(int args, int results, int error_func) //
{
	return Glua->PCall(args, results, error_func);
}
void LuaPushCFunction(void* int_ptr_object) //
{
	CFunc c_func = (CFunc)int_ptr_object;
	Glua->PushCFunction(c_func);
}
CFunc LuaGetCFunction(int pos) //
{
	return Glua->GetCFunction(pos);
}
void LuaSetField(int pos, MonoString* key)
{
	//Get char array from the key
	char* p_key = mono_string_to_utf8(key);
	//Call SetField
	Glua->SetField(pos, p_key);
	//Free memmory
	mono_free(p_key);
}
void LuaPushVector(MonoObject* vec)
{
	//Get managed Vector
	MonoClass* vector_class = mono_class_from_name(mainImage, "GmodNET.Math", "Vector");
	//Get fields
	MonoClassField* x_field = mono_class_get_field_from_name(vector_class, "x");
	MonoClassField* y_field = mono_class_get_field_from_name(vector_class, "y");
	MonoClassField* z_field = mono_class_get_field_from_name(vector_class, "z");
	//Get components
	float x, y, z;
	mono_field_get_value(vec, x_field, &x);
	mono_field_get_value(vec, y_field, &y);
	mono_field_get_value(vec, z_field, &z);
	//Create vector
	Vector new_vec;
	//Set new_vec components;
	new_vec.x = x;
	new_vec.y = y;
	new_vec.z = z;
	//Push it on the stack
	Glua->PushVector(new_vec);
}
float* LuaGetVector(int pos)
{
	//Get vector
	Vector vec = Glua->GetVector(pos);
	//Create array from vector
	float* vec_array = (float*)malloc(sizeof(float)*3);
	vec_array[0] = vec.x;
	vec_array[1] = vec.y;
	vec_array[2] = vec.z;
	return vec_array;
}
//This function is a bridge of free to managed code. It's usage is really limited;
void BridgeFree(void* ptr)
{
	free(ptr);
}

//Method to register functions with Mono runtime
void RegisterFunctions()
{
	mono_add_internal_call("GmodNET.Lua::IntPushSpecial(int)", (void*)LuaPushSpecial);
	mono_add_internal_call("GmodNET.Lua::IntPush(int)", (void*)LuaPush);
	mono_add_internal_call("GmodNET.Lua::IntPushString(string)", (void*)LuaPushString);
	mono_add_internal_call("GmodNET.Lua::IntPushNumber(double)", (void*)LuaPushNumber);
	mono_add_internal_call("GmodNET.Lua::IntPushBool(int)", (void*)LuaPushBool);
	mono_add_internal_call("GmodNET.Lua::IntGetField(int,string)", (void*)LuaGetField);
	mono_add_internal_call("GmodNET.Lua::IntCall(int,int)", (void*)LuaCall);
	mono_add_internal_call("GmodNET.Lua::IntPCall(int,int,int)", (void*)LuaPCall);
	mono_add_internal_call("GmodNET.Lua::IntPop(int)", (void*)LuaPop);
	mono_add_internal_call("GmodNET.Lua::IntGetNumber(int)", (void*)LuaGetNumber);
	mono_add_internal_call("GmodNET.Lua::IntGetString(int)", (void*)LuaGetString);
	mono_add_internal_call("GmodNET.Lua::IntGetBool(int)", (void*)LuaGetBool);
	mono_add_internal_call("GmodNET.Lua::IntPushCFunction(intptr)", (void*)LuaPushCFunction);
	mono_add_internal_call("GmodNET.Lua::IntGetCFunction(int)", (void*)LuaGetCFunction);
	mono_add_internal_call("GmodNET.Lua::IntSetField(int,string)", (void*)LuaSetField);
	mono_add_internal_call("GmodNET.Lua::IntPushVector(GmodNET.Math.Vector)", (void*)LuaPushVector);
	mono_add_internal_call("GmodNET.Lua::IntGetVector(int)", (void*)LuaGetVector);

	mono_add_internal_call("GmodNET.Lua::BridgeFree(intptr)", (void*)BridgeFree);
}

//Function to setup assembly. Load main assembly, get its image.
void SetUpAssembly()
{
	//Open assembly
	GmodPrint("Loading main assembly...");
	mainAssembly = mono_domain_assembly_open(mainDomain, (GmodModuleDirectory() + "/Gmod.NET.dll").c_str());
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
	mono_set_dirs((GmodModuleDirectory() + "/lib").c_str(), (GmodModuleDirectory() + "/etc").c_str());
	
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