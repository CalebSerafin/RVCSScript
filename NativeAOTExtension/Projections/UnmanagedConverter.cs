using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NativeAOTExtension.Projections;
/// <summary> Unmanaged Helpers </summary>
internal static class UnmanagedConverter {
    public static unsafe string? StringPtrToString(byte* ptr) =>
        Marshal.PtrToStringUTF8((IntPtr)ptr);

    public static unsafe ReadOnlySpan<string?> ArgPtrsToStringSpan(byte** argPtrs, int argCount) {
        string?[] args = new string[argCount];
        for (int i = 0; i < argCount; i++) {
            byte* argPtr = argPtrs[i];
            args[i] = Marshal.PtrToStringUTF8((IntPtr)argPtr);
        }
        return args;
    }
}
