//
// Created by Gleb Krasilich on 02.10.2019.
//
#include <GarrysMod/Lua/Interface.h>
#include <GarrysMod/Lua/LuaBase.h>
#include <codecvt>
#ifdef WIN32
#include <Windows.h>
#else
#include <dlfcn.h>
#include <locale>
#endif // WIN32
#include <string>
#include "../dotnethelper-src/cleanup_function_type.h"

typedef cleanup_function_fn (*InitNetRuntime_fn)(GarrysMod::Lua::ILuaBase* lua);

cleanup_function_fn cleanup_function = nullptr;

//Invoked by Garry's Mod on module load
GMOD_MODULE_OPEN()
{
    // On Linux, modify SIGSEGV handling
#ifdef __gnu_linux__
    void *linux_helper_handle = dlopen("garrysmod/lua/bin/liblinuxhelper.so", RTLD_LAZY);
    void (*pointer_to_install_sigsegv)(void);
    pointer_to_install_sigsegv = (void(*)())dlsym(linux_helper_handle, "install_sigsegv_handler");
    pointer_to_install_sigsegv();
#endif

    // Native welcome message
    LUA->PushSpecial(GarrysMod::Lua::SPECIAL_GLOB);
    LUA->GetField(-1, "print");
    LUA->PushString((std::string("Gmod dotnet loader ") + std::string(SEM_VERSION)).c_str());
    LUA->Call(1, 0);
    LUA->Pop(1);

#ifdef WIN32
    HMODULE dotnethelper_handle = LoadLibraryA("garrysmod/lua/bin/dotnethelper.dll");
    InitNetRuntime_fn InitNetRuntime = reinterpret_cast<InitNetRuntime_fn>(GetProcAddress(dotnethelper_handle, "InitNetRuntime"));
#elif __APPLE__

#elif __gnu_linux__

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
        LUA->PushString("::error::Unable to load dotnet runtime, check standard error stream of the process.");
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
        cleanup_function();
    }

    return 0;
}