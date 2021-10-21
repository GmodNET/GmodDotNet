//
// Created by Gleb Krasilich on 11.10.2020.
//
#include <filesystem>
#include <fstream>
#include <iostream>
#include <string>
#include <GarrysMod/Lua/LuaBase.h>
#include "LuaAPIExposure.h"
#include "cleanup_function_type.h"
#include "hostfxr_interop.h"
#include "utils/path.h"
#ifdef WIN32
#include <Windows.h>
#else
#include <cstring>
#endif

#ifdef WIN32
#define DYNAMIC_EXPORT _declspec(dllexport)
#else
#define DYNAMIC_EXPORT __attribute__((visibility("default")))
#endif

typedef int (*managed_delegate_executor_fn)(lua_State* luaState);

typedef cleanup_function_fn (*managed_main_fn)(GarrysMod::Lua::ILuaBase* lua,
                                               const char* versionString,
                                               int versionStringLength,
                                               void** internalFunctionsParam,
                                               GarrysMod::Lua::CFunc native_delegate_executor_ptr,
                                               /* Out Param */ managed_delegate_executor_fn* managed_delegate_executor_ptr);

using tstring = std::basic_string<char_t>;

std::ofstream error_log_file;

managed_delegate_executor_fn managed_delegate_executor = nullptr;
managed_main_fn managed_main = nullptr;

void HOSTFXR_CALLTYPE dotnet_error_writer(const char_t* message)
{
    error_log_file << message << std::endl;
}

int native_delegate_executor(lua_State* luaState)
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

// clang-format off
void* params_to_managed_code[] = {
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
// clang-format on

extern "C" DYNAMIC_EXPORT cleanup_function_fn InitNetRuntime(GarrysMod::Lua::ILuaBase* lua)
{
    if(!error_log_file.is_open())
    {
        error_log_file.open("dotnet_loader_error.log");
    }

    if(managed_main == nullptr)
    {
        try
        {
            hostfxr_interop hostfxr{};
            hostfxr.set_error_writer(dotnet_error_writer);
            managed_main = hostfxr.load_gmodnet_main<managed_main_fn>();
        }
        catch(const std::runtime_error& ex)
        {
            error_log_file << ex.what() << std::endl;
            return nullptr;
        }
    }

    return managed_main(lua,
                        SEM_VERSION,
                        static_cast<int>(std::strlen(SEM_VERSION)),
                        params_to_managed_code,
                        native_delegate_executor,
                        &managed_delegate_executor);
}
