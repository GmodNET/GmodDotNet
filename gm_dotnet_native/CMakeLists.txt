#Specify minimal CMake version
cmake_minimum_required (VERSION 3.10)

#Set CMake build mode
set(CMAKE_BUILD_TYPE Release CACHE STRING "Build Mode")

#Visual Studio configuration (Debug/Release) to produce
set(CMAKE_CONFIGURATION_TYPES "Release;" CACHE STRING "Available build configurations for Visual Studio generator")

#C++ version to target
set(CMAKE_CXX_STANDARD 17 CACHE INTERNAL "C++ standard version" FORCE)

#Set path to the external includes header files
set(EXTERNAL_INCLUDES_PATH "${CMAKE_CURRENT_SOURCE_DIR}/external_includes/" CACHE FILEPATH "Path to external header files")


#Project name
project(gm_dotnet_native)
add_library(gm_dotnet_native SHARED "src/gm_dotnet.cpp" src/LuaAPIExposure.h src/LuaAPIExposure.cpp)
#Set up external include libraries
include_directories ("${EXTERNAL_INCLUDES_PATH}")
#Add linux helper lib
if(CMAKE_SYSTEM_NAME MATCHES "Linux")
	add_library(linuxhelper SHARED linux-helper-src/segv_signal_handler.h linux-helper-src/segv_signal_handler.cpp)
	set_target_properties(linuxhelper PROPERTIES PREFIX "" SUFFIX ".so")
endif()

if(WIN32)
	set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_win64")
	add_custom_target(create_client_lib ALL DEPENDS gm_dotnet_native COMMAND COPY /B /Y \"${CMAKE_BINARY_DIR}\\Release\\gmsv_dotnet_win64.dll\" /B /Y \"${CMAKE_BINARY_DIR}\\Release\\gmcl_dotnet_win64.dll\")
else()
	if(CMAKE_SYSTEM_NAME MATCHES "Linux")
		set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_linux64" SUFFIX ".dll" PREFIX "")
		add_custom_target(create_client_lib ALL DEPENDS gm_dotnet_native COMMAND cp -f -T ${CMAKE_BINARY_DIR}/gmsv_dotnet_linux64.dll ${CMAKE_BINARY_DIR}/gmcl_dotnet_linux64.dll)
	elseif(CMAKE_SYSTEM_NAME MATCHES "Darwin")
		set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_osx64" SUFFIX ".dll" PREFIX "")
		add_custom_target(create_client_lib ALL DEPENDS gm_dotnet_native COMMAND cp -f ${CMAKE_BINARY_DIR}/gmsv_dotnet_osx64.dll ${CMAKE_BINARY_DIR}/gmcl_dotnet_osx64.dll)
	endif()
endif()