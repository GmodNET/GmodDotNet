//
// Created by Gleb Krasilich on 02.10.2019.
//
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
#include "LuaAPIExposure.h"

#ifdef WIN32
#define STRING_FORMATER( STR ) converter.from_bytes(STR).c_str()
#else
#define STRING_FORMATER( STR ) string(STR).c_str()
#endif

using namespace std;
using namespace GarrysMod::Lua;

int maj_ver = 0;
int min_ver = 5;
int misc_ver = 2;

wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;

hostfxr_initialize_for_runtime_config_fn hostfxr_initialize_for_runtime_config = nullptr;
hostfxr_handle host_fxr_handle = nullptr;
hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate = nullptr;
hostfxr_close_fn hostfxr_close = nullptr;

typedef void (*cleanup_delegate_fn)();
cleanup_delegate_fn cleanup_delegate = nullptr;


//Invoked by Garry's Mod on module load
GMOD_MODULE_OPEN()
{
    char game_char_buffer [300];
    #ifdef WIN32
    int game_path_length = GetModuleFileNameA(nullptr, game_char_buffer, 299);
    #else
    int game_path_length = readlink("/proc/self/exe", game_char_buffer, 299);
    game_char_buffer[game_path_length] = '\0';
    #endif
    char dotnet_folder [] = "garrysmod/lua/bin/dotnet";
    #ifdef WIN32
    wstring game_char_buffer_formated = converter.from_bytes(game_char_buffer);
    wstring dotnet_folder_formated = converter.from_bytes(dotnet_folder);
    hostfxr_initialize_parameters runtime_params = {sizeof(hostfxr_initialize_parameters), game_char_buffer_formated.c_str(), dotnet_folder_formated.c_str()};
    #else
    hostfxr_initialize_parameters runtime_params = {sizeof(hostfxr_initialize_parameters), game_char_buffer, dotnet_folder};
    #endif
    void * hostfxr_pointer = nullptr;
    #ifdef WIN32
    hostfxr_pointer = LoadLibraryA("garrysmod/lua/bin/dotnet/host/fxr/3.1.1/hostfxr.dll");
    #elif __APPLE__
    hostfxr_pointer = dlopen("garrysmod/lua/bin/dotnet/host/fxr/3.1.1/libhostfxr.dylib", RTLD_LAZY);
    #else
    hostfxr_pointer = dlopen("garrysmod/lua/bin/dotnet/host/fxr/3.1.1/libhostfxr.so", RTLD_LAZY);
    #endif
    if(hostfxr_pointer == nullptr)
    {
         fprintf(stderr, "Unable to load hostfxr! \n");
         return 0;
    }

    #ifdef WIN32
    hostfxr_initialize_for_runtime_config = (hostfxr_initialize_for_runtime_config_fn)GetProcAddress((HMODULE)hostfxr_pointer, "hostfxr_initialize_for_runtime_config");
    #else
    hostfxr_initialize_for_runtime_config = (hostfxr_initialize_for_runtime_config_fn)dlsym(hostfxr_pointer, "hostfxr_initialize_for_runtime_config");
    #endif
    if(hostfxr_initialize_for_runtime_config == nullptr)
    {
        fprintf(stderr, "Unable to locate hostfxr_initialize_for_runtime_config function!");
        return 0;
    }
    hostfxr_initialize_for_runtime_config(STRING_FORMATER("garrysmod/lua/bin/gmodnet/GmodNET.runtimeconfig.json"), &runtime_params, &host_fxr_handle);
    if(host_fxr_handle == nullptr)
    {
        fprintf(stderr, "Unable to create hostfxr handle!");
        return 0;
    }

    #ifdef WIN32
    hostfxr_get_runtime_delegate = (hostfxr_get_runtime_delegate_fn)GetProcAddress((HMODULE)hostfxr_pointer, "hostfxr_get_runtime_delegate");
    #else
    hostfxr_get_runtime_delegate = (hostfxr_get_runtime_delegate_fn)dlsym(hostfxr_pointer, "hostfxr_get_runtime_delegate");
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

    typedef cleanup_delegate_fn (*managed_delegate_fn)(ILuaBase * lua_base, int maj_ver, int min_ver, int misc_ver, void ** params);
    managed_delegate_fn managed_delegate = nullptr;

    typedef void (*resolver_helper_delegate_fn)();
    resolver_helper_delegate_fn resolver_helper_delegate = nullptr;

    get_function_pointer(STRING_FORMATER("garrysmod/lua/bin/gmodnet/DefaultContextResolver.dll"),
            STRING_FORMATER("GmodNET.Resolver.DefaultContextResolver, DefaultContextResolver"), STRING_FORMATER("Main"),
            STRING_FORMATER("GmodNET.Resolver.MainDelegate, DefaultContextResolver"), nullptr, (void**)&resolver_helper_delegate);

    if(resolver_helper_delegate == nullptr)
    {
        fprintf(stderr, "Unable to get resolver helper delegate! \n");
        return 0;
    }

    resolver_helper_delegate();

    get_function_pointer(STRING_FORMATER("garrysmod/lua/bin/gmodnet/GmodNET.dll"), STRING_FORMATER("GmodNET.Startup, GmodNET"),
                         STRING_FORMATER("Main"), STRING_FORMATER("GmodNET.MainDelegate, GmodNET"), nullptr, (void**)&managed_delegate);
    if(managed_delegate == nullptr)
    {
        fprintf(stderr, "Unable to get managed delegate! \n");
        return 0;
    }

    // Parameters to pass to managed code
    void * params_to_managed_code[] = {
            (void*)export_top,
            (void*)export_push,
            (void*)export_pop,
            (void*)export_get_field,
            (void*)export_set_field,
            (void*)export_create_table,
            (void*)export_set_metatable,
            (void*)export_get_metatable,
            (void*)export_call,
            (void*)export_p_call,
            (void*)exports_equal,
            (void*)export_raw_equal,
            (void*)export_insert,
            (void*)export_remove,
            (void*)export_next,
            (void*)export_throw_error,
            (void*)export_check_type,
            (void*)export_arg_error,
            (void*)export_get_string,
            (void*)export_get_number,
            (void*)export_get_bool,
            (void*)export_get_c_function,
            (void*)export_push_nil,
            (void*)export_push_string,
            (void*)export_push_number,
            (void*)export_push_bool,
            (void*)export_push_c_function,
            (void*)export_push_c_closure,
            (void*)export_reference_create,
            (void*)export_reference_free,
            (void*)export_reference_push,
            (void*)export_push_special,
            (void*)export_is_type,
            (void*)export_get_type,
            (void*)export_get_type_name,
            (void*)export_obj_len,
            (void*)export_get_angle,
            (void*)export_get_vector,
            (void*)export_push_angle,
            (void*)export_push_vector,
            (void*)export_set_state,
            (void*)export_create_metatable,
            (void*)export_push_metatable,
            (void*)export_push_user_type,
            (void*)export_set_user_type,
            (void*)export_get_user_type,
            (void*)export_get_iluabase_from_the_lua_state,
            (void*)export_get_table,
            (void*)export_set_table,
            (void*)export_raw_get,
            (void*)export_raw_set,
            (void*)export_push_user_data,
            (void*)export_check_string,
            (void*)export_check_number
    };

    cleanup_delegate = managed_delegate(LUA, maj_ver, min_ver, misc_ver, params_to_managed_code);

    if(cleanup_delegate == nullptr)
    {
        fprintf(stderr, "Managed runtime returned NULL cleanup_delegate pointer \n");
    }

    return 0;
}

//Invoked by Garry's Mod on module unload
GMOD_MODULE_CLOSE()
{
    cleanup_delegate();
    cleanup_delegate = nullptr;

    #ifdef WIN32
    HMODULE hostfxr_lib = LoadLibraryA("garrysmod/lua/bin/dotnet/host/fxr/3.1.0/hostfxr.dll");
    hostfxr_close = (hostfxr_close_fn)GetProcAddress(hostfxr_lib, "hostfxr_close");
    #elif __APPLE__
    void * hostfxr_lib = dlopen("garrysmod/lua/bin/dotnet/host/fxr/3.1.0/libhostfxr.dylib", RTLD_LAZY);
    hostfxr_close = (hostfxr_close_fn)dlsym(hostfxr_lib, "hostfxr_close");
    #else
    void * hostfxr_lib = dlopen("garrysmod/lua/bin/dotnet/host/fxr/3.1.0/libhostfxr.so", RTLD_LAZY);
    hostfxr_close = (hostfxr_close_fn)dlsym(hostfxr_lib, "hostfxr_close");
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