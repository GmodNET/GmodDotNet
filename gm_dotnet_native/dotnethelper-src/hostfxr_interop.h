#pragma once

#include <filesystem>
#include <functional>
#include <netcore/hostfxr.h>
#include <netcore/coreclr_delegates.h>

#ifdef WIN32
#define __T(x) L##x
#else
#define __T(x) x
#endif

#define _T(x) __T(x)

class hostfxr_interop
{
public:
    explicit hostfxr_interop();

    template<typename T>
    T load_gmodnet_main();

    hostfxr_set_error_writer_fn set_error_writer{};

private:
    template<typename T>
    T validate_call(const std::function<int(T*)>& function_call, const std::string& result_name, const std::string& error_msg);
    hostfxr_handle init_runtime_for_gmodnet();
    get_function_pointer_fn managed_function_pointer_getter();

private:
    hostfxr_initialize_for_dotnet_command_line_fn initialize_for_dotnet_command_line{};
    hostfxr_get_runtime_delegate_fn get_runtime_delegate{};
};

#include "hostfxr_interop_p.h"
