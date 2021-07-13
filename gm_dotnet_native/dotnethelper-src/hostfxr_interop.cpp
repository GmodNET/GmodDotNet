#include "hostfxr_interop.h"
#include <type_traits>
#include <dynalo/dynalo.hpp>
#include "utils/path.h"

#ifdef WIN32
#define STRINGIZE _CRT_STRINGIZE
#else
#define STRINGIZE_(x) #x
#define STRINGIZE(x) STRINGIZE_(x)
#endif

const std::filesystem::path dotnet_folder("dotnet");
const std::filesystem::path gmodnet_dll_path("gmodnet/GmodNET.dll");
const dynalo::library hostfxr_library(utils::path::lua_bin_folder() / _T("dotnet/host/fxr") / NET_CORE_VERSION
                                      / dynalo::to_native_name("hostfxr"));

hostfxr_interop::hostfxr_interop()
{
#define LOAD_FN(x) x = hostfxr_library.get_function<std::remove_pointer_t<hostfxr_##x##_fn>>(STRINGIZE(hostfxr_##x))

    LOAD_FN(initialize_for_dotnet_command_line);
    LOAD_FN(get_runtime_delegate);
    LOAD_FN(set_error_writer);

#undef LOAD_FN
}

hostfxr_handle hostfxr_interop::init_runtime_for_gmodnet()
{
    const auto dotnet_root_path = (std::filesystem::current_path() / utils::path::lua_bin_folder() / dotnet_folder).make_preferred();
    std::filesystem::path game_exe_path = utils::path::get_exe();

    hostfxr_initialize_parameters dotnet_runtime_params;
    dotnet_runtime_params.size = sizeof(hostfxr_initialize_parameters);
    dotnet_runtime_params.host_path = game_exe_path.c_str();
    dotnet_runtime_params.dotnet_root = dotnet_root_path.c_str();

    const auto gmodnet_dll_relative_path = utils::path::lua_bin_folder() / gmodnet_dll_path;
    const char_t* dotnet_args[] = {_T("exec"), gmodnet_dll_relative_path.c_str()};

    return validate_call<hostfxr_handle>(
        [&](auto* result) {
            return initialize_for_dotnet_command_line(static_cast<int>(std::size(dotnet_args)), dotnet_args, &dotnet_runtime_params, result);
        },
        "hostfxr_handle",
        "Unable to initialize dotnet runtime");
}

get_function_pointer_fn hostfxr_interop::managed_function_pointer_getter()
{
    hostfxr_handle runtime_environment_handle = init_runtime_for_gmodnet();
    return validate_call<get_function_pointer_fn>(
        [&](auto* result) {
            return get_runtime_delegate(runtime_environment_handle, hdt_get_function_pointer, reinterpret_cast<void**>(result));
        },
        "get_function_pointer",
        "Unable to get delegate of dotnet runtime");
}
