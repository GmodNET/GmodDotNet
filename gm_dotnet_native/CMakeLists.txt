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

# Set path to includes
set (EXTERN_INCLUDE_DIR "${CMAKE_CURRENT_SOURCE_DIR}/includes/" CACHE FILEPATH "Path to external header files (mono and Garry's mod)")

# Set up project settings
project(gm_dotnet_native)
add_library (gm_dotnet_native SHARED "src/gm_dotnet.cpp" "src/gprint.h" "src/gprint.cpp" "src/version.h")
include_directories ("${EXTERN_INCLUDE_DIR}")

# Check if you want to build x64 module
set(64_BUILD false CACHE BOOL "Build as 64-bit shared library" FORCE)

# Check if we are building for Windows
if (WIN32)
	# Check if Visual Studio generator was set to produce 64-bit build
	if (CMAKE_SIZEOF_VOID_P EQUAL 8)
		set(64_BUILD true CACHE BOOL "Build as 64-bit shared library" FORCE)
	endif()
	# Check if we are building x32 module
	if (NOT 64_BUILD)
		# Set output file name
		set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_win32")
		# Set static libraries to link with
		target_link_libraries(gm_dotnet_native "${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/libmono-static-sgen.lib" "Bcrypt.lib" 
		"Mswsock.lib" "Ws2_32.lib" "Mincore.lib" "${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/MonoPosixHelper.lib" "Winmm.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/gtest.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/gtest_main.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAArch64AsmParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAArch64AsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAArch64CodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAArch64Desc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAArch64Disassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAArch64Info.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAArch64Utils.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAnalysis.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMARMAsmParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMARMAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMARMCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMARMDesc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMARMDisassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMARMInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAsmParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMBitReader.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMBitWriter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMCore.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMCppBackendCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMCppBackendInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMDebugInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMExecutionEngine.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMHexagonAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMHexagonCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMHexagonDesc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMHexagonInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMInstCombine.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMInstrumentation.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMInterpreter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMipa.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMipo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMIRReader.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMJIT.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMLineEditor.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMLinker.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMLTO.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMC.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMCAnalysis.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMCDisassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMCJIT.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMCParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMipsAsmParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMipsAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMipsCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMipsDesc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMipsDisassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMipsInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMSP430AsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMSP430CodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMSP430Desc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMMSP430Info.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMNVPTXAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMNVPTXCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMNVPTXDesc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMNVPTXInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMObjCARCOpts.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMObject.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMOption.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMPowerPCAsmParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMPowerPCAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMPowerPCCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMPowerPCDesc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMPowerPCDisassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMPowerPCInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMProfileData.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMR600AsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMR600CodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMR600Desc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMR600Info.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMRuntimeDyld.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMScalarOpts.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSelectionDAG.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSparcAsmParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSparcAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSparcCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSparcDesc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSparcDisassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSparcInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSupport.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSystemZAsmParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSystemZAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSystemZCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSystemZDesc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSystemZDisassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMSystemZInfo.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMTableGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMTarget.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMTransformUtils.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMVectorize.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMX86AsmParser.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMX86AsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMX86CodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMX86Desc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMX86Disassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMX86Info.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMX86Utils.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMXCoreAsmPrinter.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMXCoreCodeGen.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMXCoreDesc.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMXCoreDisassembler.lib"
		"${CMAKE_CURRENT_SOURCE_DIR}/static_libs/32/LLVMXCoreInfo.lib")
	# If we are building x64 module...
	else()
		# Set output file name
		set_target_properties(gm_dotnet_native PROPERTIES OUTPUT_NAME "gmsv_dotnet_win64")
		# Set static libraries to link with
		target_link_libraries(gm_dotnet_native "${CMAKE_CURRENT_SOURCE_DIR}/static_libs/64/libmono-static-sgen.lib" "Bcrypt.lib" 
		"Mswsock.lib" "Ws2_32.lib" "Mincore.lib")
	endif()
endif()