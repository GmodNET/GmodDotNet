#Specify minimal CMake version
cmake_minimum_required (VERSION 3.13)

#Set CMake build mode
set(CMAKE_BUILD_TYPE Release CACHE STRING "Build Mode")

#Visual Studio configuration (Debug/Release) to produce
set(CMAKE_CONFIGURATION_TYPES "Release;" CACHE STRING "Available build configurations for Visual Studio generator")

#C++ version to target
set(CMAKE_CXX_STANDARD 17 CACHE INTERNAL "C++ standard version" FORCE)

#Set path to the external includes header files
set(EXTERNAL_INCLUDES_PATH "${CMAKE_CURRENT_SOURCE_DIR}/external_includes/" CACHE FILEPATH "Path to external header files")

#Set git commit variable
set(SEM_VERSION "no_version" CACHE STRING "SemVer of native part")

#Project name
project(gm_dotnet_native)
add_library(gm_dotnet_native SHARED src/gm_dotnet.cpp src/LuaAPIExposure.h src/LuaAPIExposure.cpp dotnethelper-src/cleanup_function_type.h)
add_library(dotnethelper SHARED dotnethelper-src/dotnethelper.cpp dotnethelper-src/cleanup_function_type.h)
#Set up external include libraries
include_directories ("${EXTERNAL_INCLUDES_PATH}")
#Set up compile definitions
target_compile_definitions(gm_dotnet_native PUBLIC SEM_VERSION="${SEM_VERSION}")
#Set path for output
set_target_properties(gm_dotnet_native PROPERTIES LIBRARY_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../build
		RUNTIME_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../build)
#Add linux helper lib
if(CMAKE_SYSTEM_NAME MATCHES "Linux")
	add_library(linuxhelper SHARED linux-helper-src/segv_signal_handler.h linux-helper-src/segv_signal_handler.cpp)
	set_target_properties(linuxhelper PROPERTIES LIBRARY_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../build
			RUNTIME_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../build)
endif()

if(WIN32)
	set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_win64")
else()
	if(CMAKE_SYSTEM_NAME MATCHES "Linux")
		set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_linux64" SUFFIX ".dll" PREFIX "")
	elseif(CMAKE_SYSTEM_NAME MATCHES "Darwin")
		set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_osx64" SUFFIX ".dll" PREFIX "")
	endif()
endif()