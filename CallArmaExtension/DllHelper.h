#pragma once
#include <windows.h>

class ProcPtr {
public:
    explicit ProcPtr(FARPROC ptr);

    template <typename T, typename = std::enable_if_t<std::is_function_v<T>>>
    operator T* () const {
        return reinterpret_cast<T*>(_ptr);
    }

private:
    FARPROC _ptr;
};

class DllHelper {
public:
    explicit DllHelper(LPCTSTR filename);

    ~DllHelper();

    ProcPtr operator[](LPCSTR proc_name) const;

    static HMODULE _parent_module;

private:
    HMODULE _module;
};

