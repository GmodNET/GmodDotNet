//
// Created by Gleb Krasilich on 11.10.2020.
//
#include <iostream>
#include <netcore/hostfxr.h>
#include <GarrysMod/Lua/LuaBase.h>
#include "cleanup_function_type.h"
#ifdef WIN32
#include <Windows.h>
#else
#include <dlfcn.h>
#endif

#ifdef WIN32
#define DYNANAMIC_EXPORT _declspec(dllexport)
#else
#define DYNANAMIC_EXPORT __attribute__((visibility("default")))
#endif

typedef int(*get_function_pointer_fn)(
        char* typeName,
        char* methodName,
        char* delegateTypeName,
        void* loadContext,
        void* reserved,
        void* outFunctionPointer
        );

typedef cleanup_function_fn(*managed_main_fn)(
        GarrysMod::Lua::ILuaBase* lua,
        char* versionString,
        int versionStringLength,
        void** internalFunctionsParam
        );

managed_main_fn managed_main = nullptr;

#ifdef WIN32
void* hostfxr_library_handle = LoadLibraryA("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.1.20451.14/hostfxr.dll");
#elif __APPLE__
hostfxr_pointer = dlopen("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.1.20451.14/libhostfxr.dylib", RTLD_LAZY);
#elif __gnu_linux__
void* hostfxr_library_handle = dlopen("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.1.20451.14/libhostfxr.so", RTLD_LAZY);
#endif

#ifdef WIN32
hostfxr_initialize_for_dotnet_command_line_fn hostfxr_initialize_for_dotnet_command_line =
        reinterpret_cast<hostfxr_initialize_for_dotnet_command_line_fn>(GetProcAddress(static_cast<HMODULE>(hostfxr_library_handle),
                                                                                       "hostfxr_initialize_for_dotnet_command_line"));
hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate =
        reinterpret_cast<hostfxr_get_runtime_delegate_fn>(GetProcAddress(static_cast<HMODULE>(hostfxr_library_handle),
                                                                         "hostfxr_get_runtime_delegate"));
#else
hostfxr_initialize_for_dotnet_command_line_fn hostfxr_initialize_for_dotnet_command_line =
        reinterpret_cast<hostfxr_initialize_for_dotnet_command_line_fn>(dlsym(hostfxr_library_handle, "hostfxr_initialize_for_dotnet_command_line"));
hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate =
        reinterpret_cast<hostfxr_get_runtime_delegate_fn>(dlsym(hostfxr_library_handle, "hostfxr_get_runtime_delegate"));
#endif

extern "C" DYNANAMIC_EXPORT void* InitNetRuntime()
{
    if(managed_main == nullptr)
    {
        if(hostfxr_initialize_for_dotnet_command_line == nullptr || hostfxr_get_runtime_delegate == nullptr)
        {
            std::cerr << "Unable to load hostfxr library" << std::endl;
            return nullptr;
        }

        hostfxr_handle* runtime_environment_handle = nullptr;

#ifdef WIN32
        const wchar_t* dotnet_args[1] = {L"garrysmod/lua/bin/gmodnet/GmodNET.dll"};
#else
        const char* dotnet_args[1] = {"garrysmod/lua/bin/gmodnet/GmodNET.dll"};
#endif
        int init_success_code = hostfxr_initialize_for_dotnet_command_line(1, dotnet_args, nullptr, runtime_environment_handle);
        if(init_success_code != 0)
        {
            std::cerr << "Unable to initialize dotnet runtime. Error code: " << init_success_code << std::endl;
            return nullptr;
        }
        if(runtime_environment_handle == nullptr)
        {
            std::cerr << "runtime_environment_handle is null" << std::endl;
        }
        get_function_pointer_fn get_function_pointer = nullptr;
        int get_runtime_delegate_success_code =
                hostfxr_get_runtime_delegate(runtime_environment_handle, hdt_get_function_pointer, reinterpret_cast<void**>(&get_function_pointer));
        if(get_runtime_delegate_success_code != 0)
        {
            std::cerr << "Unable to get delegate of dotnet runtime. Error code: " << get_runtime_delegate_success_code << std::endl;
            return nullptr;
        }
        if(get_function_pointer == nullptr)
        {
            std::cerr << "get_function_pointer is null" << std::endl;
            return nullptr;
        }
    }
    return nullptr;
}

