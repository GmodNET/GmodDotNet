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
#include <filesystem>
#ifdef WIN32
#include <Windows.h>
#else
#include <cstring>
#include <dlfcn.h>
#include <unistd.h>
#endif

#ifdef WIN32
#define DYNAMIC_EXPORT _declspec(dllexport)
#define __T(x) L ## x
#else
#define DYNAMIC_EXPORT __attribute__((visibility("default")))
#define __T(x) x
#endif

#define _T(x) __T(x)

typedef int (*managed_delegate_executor_fn)(
        lua_State * luaState
);

typedef cleanup_function_fn(*managed_main_fn)(
        GarrysMod::Lua::ILuaBase* lua,
        const char* versionString,
        int versionStringLength,
        void** internalFunctionsParam,
        GarrysMod::Lua::CFunc native_delegate_executor_ptr,
        /* Out Param */ managed_delegate_executor_fn* managed_delegate_executor_ptr
        );

using tstring = std::basic_string<char_t>;

std::ofstream error_log_file;

managed_delegate_executor_fn managed_delegate_executor = nullptr;

managed_main_fn managed_main = nullptr;

const std::filesystem::path bin_folder = _T("garrysmod/lua/bin");
const std::filesystem::path hostfxr_path = (bin_folder / _T("dotnet/host/fxr") / NET_CORE_VERSION).make_preferred();
#ifdef WIN32
HMODULE hostfxr_library_handle = LoadLibraryW((hostfxr_path / _T("hostfxr.dll")).c_str());
#elif __APPLE__
void* hostfxr_library_handle = dlopen((hostfxr_path / "libhostfxr.dylib").c_str(), RTLD_LAZY | RTLD_LOCAL);
#elif __gnu_linux__
void* hostfxr_library_handle = dlopen((hostfxr_path / "libhostfxr.so").c_str(), RTLD_LAZY);
#endif

template<typename T>
bool LoadFunction(const char* function_name, T& out_func)
{
#ifdef WIN32
    out_func = reinterpret_cast<T>(GetProcAddress(hostfxr_library_handle, function_name));
#else
    out_func = reinterpret_cast<T>(dlsym(hostfxr_library_handle, function_name));
#endif
    return (out_func != nullptr);
}
hostfxr_initialize_for_dotnet_command_line_fn hostfxr_initialize_for_dotnet_command_line = nullptr;
hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate = nullptr;
hostfxr_set_error_writer_fn hostfxr_set_error_writer = nullptr;

void HOSTFXR_CALLTYPE dotnet_error_writer(const char_t *message)
{
    error_log_file << message << std::endl;
}

int native_delegate_executor(lua_State * luaState)
{
    int return_val = managed_delegate_executor(luaState);

    if(return_val >= 0)
    {
        return return_val;
    }
    else
    {
        const char* error_message = luaState->luabase->GetString(-1);
        luaState->luabase->ThrowError(error_message);
        return 0;
    }
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

extern "C" DYNAMIC_EXPORT cleanup_function_fn InitNetRuntime(GarrysMod::Lua::ILuaBase* lua)
{
    if(!error_log_file.is_open())
    {
        error_log_file.open("dotnet_loader_error.log");
    }

    if(managed_main == nullptr)
    {
        if(!(LoadFunction("hostfxr_initialize_for_dotnet_command_line", hostfxr_initialize_for_dotnet_command_line)
            && LoadFunction("hostfxr_get_runtime_delegate", hostfxr_get_runtime_delegate)
            && LoadFunction("hostfxr_set_error_writer", hostfxr_set_error_writer)))
        {
            error_log_file << "Unable to load hostfxr library" << std::endl;
            return nullptr;
        }

        hostfxr_handle runtime_environment_handle;

        hostfxr_set_error_writer(dotnet_error_writer);

        const auto gmodnet_dll_relative_path = bin_folder / _T("gmodnet/GmodNET.dll");
        const auto dotnet_root_path = (std::filesystem::current_path() / bin_folder / "dotnet").make_preferred();

        const char_t* dotnet_args[] = {_T("exec"), gmodnet_dll_relative_path.c_str()};
        
        tstring game_exe_path(301, _T('\0'));
#ifdef WIN32
        GetModuleFileNameW(nullptr, game_exe_path.data(), static_cast<DWORD>(game_exe_path.size()) - 1);
#else
        readlink("/proc/self/exe", game_exe_path.data(), game_exe_path.size() - 1);
#endif
        hostfxr_initialize_parameters dotnet_runtime_params;
        dotnet_runtime_params.size = sizeof(hostfxr_initialize_parameters);
        dotnet_runtime_params.host_path = game_exe_path.c_str();
        dotnet_runtime_params.dotnet_root = dotnet_root_path.c_str();

        int init_success_code = hostfxr_initialize_for_dotnet_command_line(static_cast<int>(std::size(dotnet_args)), dotnet_args, 
                                                                           &dotnet_runtime_params, &runtime_environment_handle);
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

        int get_managed_main_success_code = get_function_pointer(_T("GmodNET.Startup, GmodNET"), _T("Main"), UNMANAGEDCALLERSONLY_METHOD,
                                                                 nullptr, nullptr, reinterpret_cast<void**>(&managed_main));
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

    return managed_main(lua, SEM_VERSION, static_cast<int>(std::strlen(SEM_VERSION)), params_to_managed_code,
                        native_delegate_executor, &managed_delegate_executor);
}

