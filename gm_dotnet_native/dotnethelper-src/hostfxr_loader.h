#pragma once

#include <netcore/hostfxr.h>
#include <dynalo/dynalo.hpp>

#ifdef WIN32
#define STRINGIZE _CRT_STRINGIZE
#else
#define _STRINGIZE_(x) #x
#define STRINGIZE(x) STRINGIZE_(x)
#endif

struct hostfxr_functions
{
    hostfxr_initialize_for_dotnet_command_line_fn initialize_for_dotnet_command_line{};
    hostfxr_get_runtime_delegate_fn get_runtime_delegate{};
    hostfxr_set_error_writer_fn set_error_writer{};

    explicit hostfxr_functions(const dynalo::library& hostfxr_library)
    {
#define LOAD_FN(x) x = hostfxr_library.get_function<std::remove_pointer_t<hostfxr_##x##_fn>>(STRINGIZE(hostfxr_##x))

        LOAD_FN(initialize_for_dotnet_command_line);
        LOAD_FN(get_runtime_delegate);
        LOAD_FN(set_error_writer);

#undef LOAD_FN
    }
};
