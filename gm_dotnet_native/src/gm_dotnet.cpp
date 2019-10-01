#include<GarrysMod/Lua/Interface.h>
#include<GarrysMod/Lua/LuaBase.h>
#ifdef WIN32
#include <Windows.h>
#include <direct.h>
#else
#include <dlfcn.h>
#include <unistd.h>
#endif // WIN32
#include <string>


using namespace std;
using namespace GarrysMod::Lua;

//Define pointers for the CoreCLR hosting

//Pointer to the coreclr_initialize function
typedef int (*coreclr_initialize_ptr)(const char* exePath, const char* AppDomainFriendlyName, int PropertyCount, const char** propertyKeys,
	const char** propertyValues, void** hostHandle, unsigned int* domainId);

//Pointer to the coreclr_shutdown function
typedef int (*coreclr_shutdown_ptr)(void* hostHandle, unsigned int domainId);

//Pointer to the coreclr_create_delegate function
typedef int (*coreclr_create_delegate_ptr)(void* hostHandle, unsigned int domainId, const char* entryPointAssemblyName, const char* entryPointTypeName,
	const char* entryPointMethodName, void** delegate);

//Function pointers

coreclr_initialize_ptr coreclr_initialize = nullptr;
coreclr_shutdown_ptr coreclr_shutdown = nullptr;
coreclr_create_delegate_ptr coreclr_create_delegate = nullptr;

//Pointer to the CoreCLR library
void* coreclr_ptr = nullptr;

//Id of the main domain
unsigned int domainId = 0;
//Handle of the host;
void* hostHandle = nullptr;

//Main delegate pointer
typedef void (*delegate_ptr)();

//Invoked by Garry's Mod on module load
GMOD_MODULE_OPEN()
{
	//Get path to the working directory
	char* working_path_c_str;
#ifdef WIN32
	working_path_c_str = _getcwd(nullptr, 500);
#else
	working_path_c_str = getcwd(NULL, 500);
#endif // WIN32

	string working_path(working_path_c_str);

	free(working_path_c_str);

	//Load CoreCLR
	if (!coreclr_ptr)
	{
#ifdef WIN32
		coreclr_ptr = LoadLibrary((working_path + "/garrysmod/lua/bin/gm_dotnet_core/coreclr.dll").c_str());
#else
#ifndef __APPLE__
		coreclr_ptr = dlopen((working_path + "/garrysmod/lua/bin/gm_dotnet_core/libcoreclr.so").c_str(), RTLD_LAZY);
#else
		coreclr_ptr = dlopen((working_path + "/garrysmod/lua/bin/gm_dotnet_core/libcoreclr.dylib").c_str(), RTLD_LAZY);
#endif
#endif
	}

	//Load process symbols
	if (!coreclr_initialize)
	{
#ifdef WIN32
		coreclr_initialize = (coreclr_initialize_ptr)GetProcAddress((HMODULE)coreclr_ptr, "coreclr_initialize");
		coreclr_shutdown = (coreclr_shutdown_ptr)GetProcAddress((HMODULE)coreclr_ptr, "coreclr_shutdown");
		coreclr_create_delegate = (coreclr_create_delegate_ptr)GetProcAddress((HMODULE)coreclr_ptr, "coreclr_create_delegate");
#else
		coreclr_initialize = (coreclr_initialize_ptr)dlsym(coreclr_ptr, "coreclr_initialize");
		coreclr_shutdown = (coreclr_shutdown_ptr)dlsym(coreclr_ptr, "coreclr_shutdown");
		coreclr_create_delegate = (coreclr_create_delegate_ptr)dlsym(coreclr_ptr, "coreclr_create_delegate");
#endif
	}

	//Create list of app pathes
#ifdef WIN32
	string separator = ";";
#else
	string separator = ":";
#endif

	//Form list of runtime properties
	string list_of_app_paths = working_path + "/garrysmod/lua/bin/gm_dotnet_core/" + separator + working_path + "/garrysmod/lua/bin/GmodDotNet";
	string list_of_native_app_paths = working_path + "/garrysmod/lua/bin/gm_dotnet_core";

	const char* param_keys[] = { "APP_PATHS", "APP_NI_PATHS" };
	const char* param_values[] = { list_of_app_paths.c_str(), list_of_native_app_paths.c_str() };

	//Init CoreCLR
	coreclr_initialize((working_path + "/garrysmod/lua/bin/GmodDotNet").c_str(), "GmodDotNetDomain", 2, param_keys, param_values, &hostHandle, &domainId);

	//Get delegate for the Managed Main method
	delegate_ptr main = nullptr;
	coreclr_create_delegate(hostHandle, domainId, "GmodDotNet, Version=0.5.0", "GmodDotNet.GmodDotNet", "Main", (void**)& main);

	//Call delegate
	main();

	return 0;
}

//Invoked by Garry;s Mod on module unload
GMOD_MODULE_CLOSE()
{
	coreclr_shutdown(hostHandle, domainId);

	return 0;
}