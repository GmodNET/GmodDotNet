//
// Created by Gleb Krasilich on 2/27/20.
//

#ifndef GM_DOTNET_NATIVE_SEGV_SIGNAL_HANDLER_H
#define GM_DOTNET_NATIVE_SEGV_SIGNAL_HANDLER_H

/// Custom SIGSEGV handler for Garry's Mod
extern "C" __attribute__((__visibility__("default"))) void segv_signal_handler(int sig);

#endif //GM_DOTNET_NATIVE_SEGV_SIGNAL_HANDLER_H
