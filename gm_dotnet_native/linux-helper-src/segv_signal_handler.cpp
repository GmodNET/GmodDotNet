#include "segv_signal_handler.h"
#include <signal.h>
#include <iostream>
#include <fstream>
#include <string>
#include <ctime>
#include <execinfo.h>
#include <unistd.h>
#include <regex>

using namespace std;

bool WasHandlerSet = false;

struct sigaction original_sigaction;

struct sigaction gmodnet_sigaction;

void gmodnet_segv_handler(int sig, siginfo_t * siginfo, void * context)
{
    ofstream crash_report;

    char date_time_buffer[300];
    time_t rawtime;
    struct tm * timeinfo;
    time (&rawtime);
    timeinfo = localtime (&rawtime);
    strftime(date_time_buffer, 300, "%F-%H-%M-%S", timeinfo);
    string date_time_string = string(date_time_buffer);

    string file_path_name = string("crashes/") + string("gmodnet-sigsegv-") + date_time_string + string(".log");
    crash_report.open(file_path_name);

    crash_report << "GmodDotNet Segmentation fault crash reporter." << endl;
    crash_report << "Garry's Mod received a SIGSEGV signal, which was not handled by CoreCLR." << endl;
    crash_report << "NOTE: This report may be generated just on game quit. In this case no additional actions are required." << endl;
    crash_report << "SIGSEGV signal received at " << date_time_string << "." << endl;
    crash_report << "Time format: YEAR-MONTH-DAY-HOUR-MINUTE-SECOND." << endl << endl;

    void * stack_calls[10];
    int num_of_calls_on_stack = backtrace(stack_calls, 10);
    char ** names = backtrace_symbols(stack_calls, num_of_calls_on_stack);

    crash_report << "Most recent entries on call stack:" << endl;
    for(int i = 0; i < num_of_calls_on_stack; i++)
    {
        crash_report << names[i] << endl;
    }

    crash_report.close();

    kill(getpid(), SIGKILL);
}

extern "C" __attribute__((__visibility__("default"))) void install_sigsegv_handler()
{
    if(!WasHandlerSet)
    {
        char exe_path_buffer[300];
        int exe_path_length = readlink("/proc/self/exe", exe_path_buffer, 299);
        exe_path_buffer[exe_path_length] = '\0';

        regex file_matcher = regex(".*\\/gmod$");
        if(!regex_match(exe_path_buffer, file_matcher))
        {
            WasHandlerSet = true;
            return;
        }

        //Get current sigaction (which should be Garry's Mod one)
        sigaction(SIGSEGV, nullptr, &original_sigaction);

        //Set up sigaction for handler
        gmodnet_sigaction.sa_flags = SA_SIGINFO;
        gmodnet_sigaction.sa_sigaction = gmodnet_segv_handler;
        gmodnet_sigaction.sa_mask = original_sigaction.sa_mask;

        //Register handler
        sigaction(SIGSEGV, &gmodnet_sigaction, nullptr);

        WasHandlerSet = true;
    }
}