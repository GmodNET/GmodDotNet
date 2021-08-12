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
#include "dotnethelper-src/cleanup_function_type.h"
#include <dynalo/dynalo.hpp>

cleanup_function_fn cleanup_function = nullptr;

const std::filesystem::path lua_bin_folder("garrysmod/lua/bin");

#ifdef __gnu_linux__
const dynalo::library liblinuxhelper(lua_bin_folder / "liblinuxhelper.so");
#endif

const dynalo::library dotnethelper(lua_bin_folder / dynalo::to_native_name("dotnethelper"));

//Invoked by Garry's Mod on module load
GMOD_MODULE_OPEN()
{
    // On Linux, modify SIGSEGV handling
#ifdef __gnu_linux__
    auto pointer_to_install_sigsegv = liblinuxhelper.get_function<void()>("install_sigsegv_handler");
    pointer_to_install_sigsegv();
#endif

    // Native welcome message
    LUA->PushSpecial(GarrysMod::Lua::SPECIAL_GLOB);
    LUA->GetField(-1, "print");
    LUA->PushString("Gmod dotnet loader " SEM_VERSION);
    LUA->Call(1, 0);
    LUA->Pop(1);

    try
    {
        auto InitNetRuntime = dotnethelper.get_function<cleanup_function_fn(GarrysMod::Lua::ILuaBase*)>("InitNetRuntime");
        cleanup_function = InitNetRuntime(LUA);
    }
    catch(std::runtime_error e)
    {
        auto error_msg = std::string("::error::Unable to load dotnet helper library. ") + e.what();
        LUA->PushSpecial(GarrysMod::Lua::SPECIAL_GLOB);
        LUA->GetField(-1, "print");
        LUA->PushString(error_msg.c_str());
        LUA->Call(1, 0);
        LUA->Pop(1);
        return 0;
    }

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