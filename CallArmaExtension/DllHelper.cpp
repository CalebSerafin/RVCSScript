#include "DllHelper.h"

ProcPtr::ProcPtr(FARPROC ptr) : _ptr(ptr) {}


DllHelper::DllHelper(LPCTSTR filename) : _module(LoadLibrary(filename)) {}

DllHelper::~DllHelper() { FreeLibrary(_module); }

ProcPtr DllHelper::operator[](LPCSTR proc_name) const {
    return ProcPtr(GetProcAddress(_module, proc_name));
}