#include "segv_signal_handler.h"
#include <signal.h>
#include <iostream>
#include <fstream>
#include <string>
#include <ctime>

using namespace std;

extern "C" __attribute__((__visibility__("default"))) void segv_signal_handler(int sig)
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

    crash_report << "GmodDotNet Segmentation fault crash reporter" << endl;
    crash_report << "SIGSEGV signal received at " << date_time_string << endl;

    crash_report.close();

    // Pass control to default signal handler
    SIG_DFL(sig);
}