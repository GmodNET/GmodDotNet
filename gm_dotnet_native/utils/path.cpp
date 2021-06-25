#include "path.h"
#ifdef WIN32
#include <Windows.h>
#else
#include <unistd.h>
#endif

namespace utils::path
{

std::filesystem::path lua_bin_folder()
{
    return {"garrysmod/lua/bin"};
}

std::filesystem::path get_exe()
{
    const int max_name_length = 301;
    std::filesystem::path::string_type game_exe_path(max_name_length, '\0');
#ifdef WIN32
    GetModuleFileNameW(nullptr, game_exe_path.data(), static_cast<DWORD>(game_exe_path.size()) - 1);
#else
    int exe_path_length = readlink("/proc/self/exe", game_exe_path.data(), game_exe_path.size() - 1);
#endif
    return game_exe_path;
}
} //namespace utils
