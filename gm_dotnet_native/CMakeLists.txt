# gm_dotnet_native CMake configuration file
# Authors:
#	Gleb Krasilich (gleb@krasilich.net)
# 2019

# Set CMake minimum version
cmake_minimum_required (VERSION 3.12)

#Set build mode and C++ options
set(CMAKE_BUILD_TYPE Release CACHE STRING "Build Mode" FORCE)
set(CMAKE_CONFIGURATION_TYPES "Release;" CACHE STRING "Available build configurations for generator")
set(CMAKE_CXX_STANDARD 17 CACHE INTERNAL "C++ standard version" FORCE)

# Set pathes to libmono to link with
set(WIN32_LIBMONO_PATH "${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/libmono-static-sgen.lib" CACHE FILEPATH "Path to Windows x32 mono 
library" FORCE)
set(WIN64_LIBMONO_PATH "${CMAKE_CURRENT_SOURCE_DIR}/static_libs/64/libmono-static-sgen.lib" CACHE FILEPATH "PATH to Windows x64 mono 
library" FORCE)

# Set up project settings
project(gm_dotnet_native)
add_library (gm_dotnet_native SHARED "src/gm_dotnet.cpp" "src/gprint.cpp" "src/gprint.h")
include_directories ("includes/")

# Check if you want to build x64 module
set(64_BUILD false CACHE BOOL "Build as 64-bit shared library" FORCE)

# Check if we are building for Windows
if (WIN32)
	# Check if we are building x32 module
	if (NOT 64_BUILD)
		# Set output file name
		set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_win32")
		# Set static libraries to link with
		target_link_libraries(gm_dotnet_native "${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/libmono-static-sgen.lib")
	# If we are building x64 module...
	else()
		# Set output file name
		set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_win64")
		# Set static libraries to link with
		target_link_libraries(gm_dotnet_native "${CMAKE_CURRENT_SOURCE_DIR}/static_libs/64/libmono-static-sgen.lib")
	endif()
endif()