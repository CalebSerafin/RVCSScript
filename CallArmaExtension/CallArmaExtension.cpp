#include <iostream>
#include <string>
#include <windows.h>
#include <thread>
#include <chrono>
#include <mutex>
#include <strsafe.h>
#include "DllHelper.h"

void ErrorExit(LPTSTR lpszFunction) {
    // Retrieve the system error message for the last-error code

    LPVOID lpMsgBuf;
    LPVOID lpDisplayBuf;
    DWORD dw = GetLastError();
    FormatMessage(
        FORMAT_MESSAGE_ALLOCATE_BUFFER |
        FORMAT_MESSAGE_FROM_SYSTEM |
        FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,
        dw,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        (LPTSTR)&lpMsgBuf,
        0, NULL);

    // Display the error message and exit the process

    lpDisplayBuf = (LPVOID)LocalAlloc(LMEM_ZEROINIT,
        (lstrlen((LPCTSTR)lpMsgBuf) + lstrlen((LPCTSTR)lpszFunction) + 40) * sizeof(TCHAR));
    StringCchPrintf((LPTSTR)lpDisplayBuf,
        LocalSize(lpDisplayBuf) / sizeof(TCHAR),
        TEXT("%s failed with error %d: %s"),
        lpszFunction, dw, lpMsgBuf);
    MessageBox(NULL, (LPCTSTR)lpDisplayBuf, TEXT("Error"), MB_OK);

    LocalFree(lpMsgBuf);
    LocalFree(lpDisplayBuf);
    ExitProcess(dw);
    /*
    */
}

class ArmaExtensionApi {
    DllHelper _dll{ L".\\ArmaExtensions\\NativeAOTExtension_x64.dll" };

public:
    typedef int(WINAPI* RVExtensionRegisterCallbackProc)(void (*)(char* name, char* function, char* data));
    typedef int(WINAPI* RVExtensionVersionProc)(char* output, int outputSize);
    typedef int(WINAPI* RvExtensionProc)(char* output, int outputSize, const char* function);
    typedef int(WINAPI* RvExtensionArgsProc)(char* output, int outputSize, const char* function, const char** argv, int argc);
    RVExtensionRegisterCallbackProc RVExtensionRegisterCallback = _dll["RVExtensionRegisterCallback"];
    RVExtensionVersionProc RVExtensionVersion = _dll["RVExtensionVersion"];
    RvExtensionProc RVExtension = _dll["RVExtension"];
    RvExtensionArgsProc RVExtensionArgs = _dll["RVExtensionArgs"];
};

std::mutex RVCallbackMutex;
static void RVCallback(char* name, char* function, char* data) {
    RVCallbackMutex.lock();
    std::cout << "RVExtensionCallback: name: " << name << ", function: " << function << ", data: " << data << "\n";
    RVCallbackMutex.unlock();
}

int main() {
    std::cout << "Start\n";
    ArmaExtensionApi ArmaExtensionApi;

    int outputLength = 256;
    char* outputString = new char[outputLength] {};

    ArmaExtensionApi.RVExtensionRegisterCallback(&RVCallback);

    memset(outputString, 0, outputLength * sizeof(char));
    ArmaExtensionApi.RVExtensionVersion(outputString, outputLength);
    std::cout << "RVExtensionVersion: " << outputString << "\n";

    memset(outputString, 0, outputLength * sizeof(char));
    const char* argsArray[] = { (char*)"Hello", (char*)" World", (char*)"!" };
    ArmaExtensionApi.RVExtensionArgs(outputString, outputLength, "Hello RVExtensionArgs", (const char**)argsArray, 3);
    std::cout << "RvExtensionArgs: " << outputString << "\n";

    memset(outputString, 0, outputLength * sizeof(char));
    ArmaExtensionApi.RVExtension(outputString, outputLength, "Hello RVExtension1");
    std::cout << "RVExtension: " << outputString << "\n";

    memset(outputString, 0, outputLength * sizeof(char));
    ArmaExtensionApi.RVExtension(outputString, outputLength, "Hello RVExtension2");
    std::cout << "RVExtension: " << outputString << "\n";

    std::cout << "Awaiting 1.1 sec for Callbacks\n";
    using namespace std::chrono_literals;
    std::this_thread::sleep_for(1.1s);
    std::cout << "End\n";
}
