//
// Created by Gleb Krasilich on 11.10.2020.
//
#include <iostream>
#include <netcore/hostfxr.h>
#include <netcore/coreclr_delegates.h>
#include <GarrysMod/Lua/LuaBase.h>
#include "cleanup_function_type.h"
#include "LuaAPIExposure.h"
#include <string>
#include <fstream>
#ifdef WIN32
#include <Windows.h>
#else
#include <dlfcn.h>
#include <unistd.h>
#endif

#ifdef WIN32
#define DYNANAMIC_EXPORT _declspec(dllexport)
#else
#define DYNANAMIC_EXPORT __attribute__((visibility("default")))
#endif

typedef cleanup_function_fn(*managed_main_fn)(
        GarrysMod::Lua::ILuaBase* lua,
        const char* versionString,
        int versionStringLength,
        void** internalFunctionsParam
        );

std::ofstream error_log_file;

managed_main_fn managed_main = nullptr;

#ifdef WIN32
void* hostfxr_library_handle = LoadLibraryA("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.2.20475.17/hostfxr.dll");
#elif __APPLE__
void* hostfxr_library_handle = dlopen("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.2.20475.17/libhostfxr.dylib", RTLD_LAZY);
#elif __gnu_linux__
void* hostfxr_library_handle = dlopen("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.2.20475.17/libhostfxr.so", RTLD_LAZY);
#endif

#ifdef WIN32
hostfxr_initialize_for_dotnet_command_line_fn hostfxr_initialize_for_dotnet_command_line =
        reinterpret_cast<hostfxr_initialize_for_dotnet_command_line_fn>(GetProcAddress(static_cast<HMODULE>(hostfxr_library_handle),
                                                                                       "hostfxr_initialize_for_dotnet_command_line"));
hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate =
        reinterpret_cast<hostfxr_get_runtime_delegate_fn>(GetProcAddress(static_cast<HMODULE>(hostfxr_library_handle),
                                                                         "hostfxr_get_runtime_delegate"));

hostfxr_set_error_writer_fn hostfxr_set_error_writer =
        reinterpret_cast<hostfxr_set_error_writer_fn>(GetProcAddress(static_cast<HMODULE>(hostfxr_library_handle),
                                                                     "hostfxr_set_error_writer"));
#else
hostfxr_initialize_for_dotnet_command_line_fn hostfxr_initialize_for_dotnet_command_line =
        reinterpret_cast<hostfxr_initialize_for_dotnet_command_line_fn>(dlsym(hostfxr_library_handle, "hostfxr_initialize_for_dotnet_command_line"));
hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate =
        reinterpret_cast<hostfxr_get_runtime_delegate_fn>(dlsym(hostfxr_library_handle, "hostfxr_get_runtime_delegate"));
hostfxr_set_error_writer_fn hostfxr_set_error_writer =
        reinterpret_cast<hostfxr_set_error_writer_fn>(dlsym(hostfxr_library_handle, "hostfxr_set_error_writer"));
#endif

void HOSTFXR_CALLTYPE dotnet_error_writer(const char_t *message)
{
    error_log_file << message << std::endl;
}

void * params_to_managed_code[] = {
        reinterpret_cast<void*>(export_top),
        reinterpret_cast<void*>(export_push),
        reinterpret_cast<void*>(export_pop),
        reinterpret_cast<void*>(export_get_field),
        reinterpret_cast<void*>(export_set_field),
        reinterpret_cast<void*>(export_create_table),
        reinterpret_cast<void*>(export_set_metatable),
        reinterpret_cast<void*>(export_get_metatable),
        reinterpret_cast<void*>(export_call),
        reinterpret_cast<void*>(export_p_call),
        reinterpret_cast<void*>(exports_equal),
        reinterpret_cast<void*>(export_raw_equal),
        reinterpret_cast<void*>(export_insert),
        reinterpret_cast<void*>(export_remove),
        reinterpret_cast<void*>(export_next),
        reinterpret_cast<void*>(export_throw_error),
        reinterpret_cast<void*>(export_check_type),
        reinterpret_cast<void*>(export_arg_error),
        reinterpret_cast<void*>(export_get_string),
        reinterpret_cast<void*>(export_get_number),
        reinterpret_cast<void*>(export_get_bool),
        reinterpret_cast<void*>(export_get_c_function),
        reinterpret_cast<void*>(export_push_nil),
        reinterpret_cast<void*>(export_push_string),
        reinterpret_cast<void*>(export_push_number),
        reinterpret_cast<void*>(export_push_bool),
        reinterpret_cast<void*>(export_push_c_function),
        reinterpret_cast<void*>(export_push_c_closure),
        reinterpret_cast<void*>(export_reference_create),
        reinterpret_cast<void*>(export_reference_free),
        reinterpret_cast<void*>(export_reference_push),
        reinterpret_cast<void*>(export_push_special),
        reinterpret_cast<void*>(export_is_type),
        reinterpret_cast<void*>(export_get_type),
        reinterpret_cast<void*>(export_get_type_name),
        reinterpret_cast<void*>(export_obj_len),
        reinterpret_cast<void*>(export_get_angle),
        reinterpret_cast<void*>(export_get_vector),
        reinterpret_cast<void*>(export_push_angle),
        reinterpret_cast<void*>(export_push_vector),
        reinterpret_cast<void*>(export_set_state),
        reinterpret_cast<void*>(export_create_metatable),
        reinterpret_cast<void*>(export_push_metatable),
        reinterpret_cast<void*>(export_push_user_type),
        reinterpret_cast<void*>(export_set_user_type),
        reinterpret_cast<void*>(export_get_user_type),
        reinterpret_cast<void*>(export_get_iluabase_from_the_lua_state),
        reinterpret_cast<void*>(export_get_table),
        reinterpret_cast<void*>(export_set_table),
        reinterpret_cast<void*>(export_raw_get),
        reinterpret_cast<void*>(export_raw_set),
        reinterpret_cast<void*>(export_push_user_data),
        reinterpret_cast<void*>(export_check_string),
        reinterpret_cast<void*>(export_check_number),
        reinterpret_cast<void*>(export_push_c_function_safe)
};

extern "C" DYNANAMIC_EXPORT cleanup_function_fn InitNetRuntime(GarrysMod::Lua::ILuaBase* lua)
{
    if(!error_log_file.is_open())
    {
        error_log_file.open("dotnet_loader_error.log");
    }

    if(managed_main == nullptr)
    {
        if(hostfxr_initialize_for_dotnet_command_line == nullptr || hostfxr_get_runtime_delegate == nullptr || hostfxr_set_error_writer == nullptr)
        {
            error_log_file << "Unable to load hostfxr library" << std::endl;
            return nullptr;
        }

        hostfxr_handle runtime_environment_handle;

        hostfxr_set_error_writer(dotnet_error_writer);

#ifdef WIN32
        const char_t* dotnet_args[2] = {L"exec", L"garrysmod/lua/bin/gmodnet/GmodNET.dll"};
#else
        const char_t* dotnet_args[2] = {"exec", "garrysmod/lua/bin/gmodnet/GmodNET.dll"};
#endif
        hostfxr_initialize_parameters dotnet_runtime_params;
        dotnet_runtime_params.size = sizeof(hostfxr_initialize_parameters);
#ifdef WIN32
        char_t game_exe_path[301];
        int game_exe_path_len = GetModuleFileNameW(nullptr, game_exe_path, 300);
#else
        char game_exe_path[301];
        int game_exe_path_len = readlink("/proc/self/exe", game_exe_path, 300);
        game_exe_path[game_exe_path_len] = '\0';
#endif
        dotnet_runtime_params.host_path = game_exe_path;
#ifdef WIN32
        dotnet_runtime_params.dotnet_root = L"garrysmod/lua/bin/dotnet";
#else
        dotnet_runtime_params.dotnet_root = "garrysmod/lua/bin/dotnet";
#endif
        int init_success_code = hostfxr_initialize_for_dotnet_command_line(2, dotnet_args, &dotnet_runtime_params, &runtime_environment_handle);
        if(init_success_code != 0)
        {
            error_log_file << "Unable to initialize dotnet runtime. Error code: " << init_success_code << std::endl;
            return nullptr;
        }
        if(runtime_environment_handle == nullptr)
        {
            error_log_file << "runtime_environment_handle is null" << std::endl;
        }
        get_function_pointer_fn get_function_pointer = nullptr;
        int get_runtime_delegate_success_code =
                hostfxr_get_runtime_delegate(runtime_environment_handle, hdt_get_function_pointer, reinterpret_cast<void**>(&get_function_pointer));
        if(get_runtime_delegate_success_code != 0)
        {
            error_log_file << "Unable to get delegate of dotnet runtime. Error code: " << get_runtime_delegate_success_code << std::endl;
            return nullptr;
        }
        if(get_function_pointer == nullptr)
        {
            error_log_file << "get_function_pointer is null" << std::endl;
            return nullptr;
        }
#ifdef WIN32
        int get_managed_main_success_code = get_function_pointer(L"GmodNET.Startup, GmodNET", L"Main", UNMANAGEDCALLERSONLY_METHOD,
                                                                 nullptr, nullptr, reinterpret_cast<void**>(&managed_main));
#else
        int get_managed_main_success_code = get_function_pointer("GmodNET.Startup, GmodNET", "Main", UNMANAGEDCALLERSONLY_METHOD,
                                                                 nullptr, nullptr, reinterpret_cast<void**>(&managed_main));
#endif
        if(get_managed_main_success_code != 0)
        {
            error_log_file << "Unable to load managed entry point: Error code: " << get_managed_main_success_code << std::endl;
            return nullptr;
        }
        if(managed_main == nullptr)
        {
            error_log_file << "Unable to load managed entry point: managed_main is null" << std::endl;
            return nullptr;
        }
    }
    return managed_main(lua, std::string(SEM_VERSION).c_str(), std::string(SEM_VERSION).length(), params_to_managed_code);
}

