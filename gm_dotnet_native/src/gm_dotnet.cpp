#include<GarrysMod/Lua/Interface.h>
#include<GarrysMod/Lua/LuaBase.h>
#include<netcore/hostfxr.h>
#include <codecvt>
#ifdef WIN32
#include <Windows.h>
#include <direct.h>
#else
#include <dlfcn.h>
#include <unistd.h>
#include<locale>
#endif // WIN32
#include <string>

using namespace std;
using namespace GarrysMod::Lua;

wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;

hostfxr_initialize_for_runtime_config_fn hostfxr_initialize_for_runtime_config = nullptr;
hostfxr_handle host_fxr_handle = nullptr;
hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate = nullptr;
hostfxr_close_fn hostfxr_close = nullptr;


//Invoked by Garry's Mod on module load
GMOD_MODULE_OPEN()
{
    char game_char_buffer [300];
    #ifdef WIN32
    int game_path_length = GetModuleFileNameA(nullptr, game_char_buffer, 299);
    #else
    int game_path_length = readlink("/proc/self/exe", game_char_buffer, 299);
    #endif
    char dotnet_folder [] = "garrysmod/lua/bin/dotnet";
    hostfxr_initialize_parameters runtime_params = {sizeof(hostfxr_initialize_parameters), (const char_t*)game_char_buffer, (const char_t*)dotnet_folder};
    void * hostfxr_pointer = nullptr;
    #ifdef WIN32
    hostfxr_pointer = LoadLibraryA("garrysmod/lua/bin/dotnet/host/fxr/3.0.0/hostfxr.dll");
    #endif
    if(hostfxr_pointer == nullptr)
    {
         fprintf(stderr, "Unable to load hostfxr! \n");
         return 0;
    }

    #ifdef WIN32
    hostfxr_initialize_for_runtime_config = (hostfxr_initialize_for_runtime_config_fn)GetProcAddress((HMODULE)hostfxr_pointer, "hostfxr_initialize_for_runtime_config");
    #endif
    if(hostfxr_initialize_for_runtime_config == nullptr)
    {
        fprintf(stderr, "Unable to locate hostfxr_initialize_for_runtime_config function!");
        return 0;
    }
    hostfxr_initialize_for_runtime_config(converter.from_bytes("garrysmod/lua/bin/GmodNET.runtimeconfig.json").c_str(), &runtime_params, &host_fxr_handle);
    if(host_fxr_handle == nullptr)
    {
        fprintf(stderr, "Unable to create hostfxr handle!");
        return 0;
    }

    #ifdef WIN32
    hostfxr_get_runtime_delegate = (hostfxr_get_runtime_delegate_fn)GetProcAddress((HMODULE)hostfxr_pointer, "hostfxr_get_runtime_delegate");
    #endif
    if(hostfxr_get_runtime_delegate == nullptr)
    {
        fprintf(stderr, "Unable to locate hostfxr_get_runtime_delegate! \n");
        return 0;
    }
    typedef void (*get_function_pointer_fn)(const char_t * assembly_path, const char_t * type_name, const char_t * method_name, const char_t * delegate_type_name,
            void * reserved, void ** delegate);
    get_function_pointer_fn get_function_pointer = nullptr;
    hostfxr_get_runtime_delegate(host_fxr_handle, hdt_load_assembly_and_get_function_pointer, (void **)&get_function_pointer);
    if(get_function_pointer == nullptr)
    {
        fprintf(stderr, "Unable to get get_function_pointer helper! \n");
        return 0;
    }

    typedef void (*managed_delegate_fn)();
    managed_delegate_fn managed_delegate = nullptr;
    get_function_pointer(converter.from_bytes("garrysmod/lua/bin/GmodNET.dll").c_str(), converter.from_bytes("GmodNET.Startup, GmodNET").c_str(), converter.from_bytes("Main").c_str(),
            converter.from_bytes("GmodNET.MainDelegate, GmodNET").c_str(), nullptr, (void**)&managed_delegate);
    if(managed_delegate == nullptr)
    {
        fprintf(stderr, "Unable to get managed delegate! \n");
        return 0;
    }

    managed_delegate();

    return 0;
}

//Invoked by Garry's Mod on module unload
GMOD_MODULE_CLOSE()
{
    #ifdef WIN32
    HMODULE hostfxr_lib = LoadLibraryA("garrysmod/lua/bin/dotnet/host/fxr/3.0.0/hostfxr.dll");
    hostfxr_close = (hostfxr_close_fn)GetProcAddress(hostfxr_lib, "hostfxr_close");
    #endif
    if(hostfxr_close == nullptr)
    {
        fprintf(stderr, "Unable to load hosfxr_close! \n");
        return 0;
    }
    hostfxr_close(host_fxr_handle);

    host_fxr_handle = nullptr;
    hostfxr_initialize_for_runtime_config = nullptr;
    hostfxr_get_runtime_delegate = nullptr;
    hostfxr_close = nullptr;

    return 0;
}