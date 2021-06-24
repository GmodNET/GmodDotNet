//
// Created by Gleb Krasilich on 02.10.2019.
//
#include <GarrysMod/Lua/Interface.h>
#include <codecvt>
#include <filesystem>
#ifdef WIN32
#include <Windows.h>
#else
#include <dlfcn.h>
#endif // WIN32
#include <string>
#include "../dotnethelper-src/cleanup_function_type.h"

typedef cleanup_function_fn (*InitNetRuntime_fn)(GarrysMod::Lua::ILuaBase* lua);

cleanup_function_fn cleanup_function = nullptr;

//Invoked by Garry's Mod on module load
GMOD_MODULE_OPEN()
{
    const std::filesystem::path bin_folder("garrysmod/lua/bin");

    // On Linux, modify SIGSEGV handling
#ifdef __gnu_linux__
    void *linux_helper_handle = dlopen((bin_folder / "liblinuxhelper.so").c_str(), RTLD_LAZY);
    void (*pointer_to_install_sigsegv)(void);
    pointer_to_install_sigsegv = (void(*)())dlsym(linux_helper_handle, "install_sigsegv_handler");
    pointer_to_install_sigsegv();
#endif

    // Native welcome message
    LUA->PushSpecial(GarrysMod::Lua::SPECIAL_GLOB);
    LUA->GetField(-1, "print");
    LUA->PushString("Gmod dotnet loader " SEM_VERSION);
    LUA->Call(1, 0);
    LUA->Pop(1);

    InitNetRuntime_fn InitNetRuntime = nullptr;
    const char InitNetRuntime_fn_name[] = "InitNetRuntime";

#ifdef WIN32
    HMODULE dotnethelper_handle = LoadLibraryW((bin_folder / "dotnethelper.dll").make_preferred().c_str());
    if (dotnethelper_handle != nullptr)
        InitNetRuntime = reinterpret_cast<InitNetRuntime_fn>(GetProcAddress(dotnethelper_handle, InitNetRuntime_fn_name));
#elif __APPLE__
    void* dotnethelper_handle = dlopen((bin_folder / "libdotnethelper.dylib").c_str(), RTLD_LAZY);
#elif __gnu_linux__
    void* dotnethelper_handle = dlopen((bin_folder / "libdotnethelper.so").c_str(), RTLD_LAZY);
#endif

#ifndef WIN32
    InitNetRuntime = reinterpret_cast<InitNetRuntime_fn>(dlsym(dotnethelper_handle, InitNetRuntime_fn_name));
#endif

    if(InitNetRuntime == nullptr)
    {
        LUA->PushSpecial(GarrysMod::Lua::SPECIAL_GLOB);
        LUA->GetField(-1, "print");
        LUA->PushString("::error::Unable to load dotnet helper library.");
        LUA->Call(1, 0);
        LUA->Pop(1);

        return 0;
    }

    cleanup_function = InitNetRuntime(LUA);

    if(cleanup_function == nullptr)
    {
        LUA->PushSpecial(GarrysMod::Lua::SPECIAL_GLOB);
        LUA->GetField(-1, "print");
        LUA->PushString("::error::Unable to load dotnet runtime, check dotnet_loader_error.log and managed_error.log files.");
        LUA->Call(1, 0);
        LUA->Pop(1);

        return 0;
    }

    return 0;
}

//Invoked by Garry's Mod on module unload
GMOD_MODULE_CLOSE()
{
    if(cleanup_function != nullptr)
    {
        cleanup_function(LUA);
    }

    return 0;
}