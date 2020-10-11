//
// Created by Gleb Krasilich on 11.10.2020.
//
#include <netcore/hostfxr.h>
#ifdef WIN32
#include <Windows.h>
#else
#include <dlfcn.h>
#include <unistd.h>
#include <locale>
#endif

#ifdef WIN32
#define DYMANAMIC_EXPORT _declspec(dllexport)
#else
#define DYMANAMIC_EXPORT __attribute__((visibility("default")))
#endif

hostfxr_handle* handle = nullptr;

#ifdef WIN32
void* hostfxr_library_handle = LoadLibraryA("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.1.20451.14/hostfxr.dll");
#elif __APPLE__
hostfxr_pointer = dlopen("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.1.20451.14/libhostfxr.dylib", RTLD_LAZY);
#elif __gnu_linux__
void* hostfxr_library_handle = dlopen("garrysmod/lua/bin/dotnet/host/fxr/5.0.0-rc.1.20451.14/libhostfxr.so", RTLD_LAZY);
#endif

extern "C" DYMANAMIC_EXPORT void* InitNetRuntime()
{
    return nullptr;
}

