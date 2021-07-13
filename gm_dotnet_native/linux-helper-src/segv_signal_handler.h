//
// Created by Gleb Krasilich on 2/27/20.
//
#pragma once

/// Custom SIGSEGV handler for Garry's Mod
extern "C" __attribute__((__visibility__("default"))) void install_sigsegv_handler();
