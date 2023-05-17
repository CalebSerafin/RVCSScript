using System.Buffers;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeAOTExtension;
public static class RVEntryPoint {
    #region RVEntryPoints
    [UnmanagedCallersOnly(EntryPoint = "RVExtensionRegisterCallback")]
    public static void RVExtensionRegisterCallback(IntPtr funcPtr) {
        callback = Marshal.GetDelegateForFunctionPointer<ExtensionCallback>(funcPtr);
        Interlocked.Increment(ref numberOfCalls);
        CallbackAfterDelay();
    }


    [UnmanagedCallersOnly(EntryPoint = "RVExtensionVersion", CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static unsafe void RvExtensionVersion(byte* outputPtr, int outputSize) {
        using SafeByteBuffer outputBuffer = new(outputPtr, outputSize);

        Interlocked.Increment(ref numberOfCalls);
        Encoding.UTF8.GetBytes("NativeAOTExtension v23.05.17").CopyTo(outputBuffer.AsSpan());
    }

    [UnmanagedCallersOnly(EntryPoint = "RVExtension", CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static unsafe void RvExtension(byte* outputPtr, int outputSize, byte* functionNamePtr) {
        using SafeByteBuffer outputBuffer = new(outputPtr, outputSize);
        string functionName = StringPtrToString(functionNamePtr) ?? "[NoFncName]";

        int callNumber = Interlocked.Increment(ref numberOfCalls);
        Encoding.UTF8.GetBytes($"function: {functionName}. This was call number {callNumber:D}.").CopyTo(outputBuffer.AsSpan());
        CallbackAfterDelay();
    }

    [UnmanagedCallersOnly(EntryPoint = "RVExtensionArgs", CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static unsafe int RvExtensionArgs(byte* outputPtr, int outputSize, byte* functionNamePtr, byte** argsPtr, int argCount) {
        using SafeByteBuffer outputBuffer = new(outputPtr, outputSize);
        string functionName = StringPtrToString(functionNamePtr) ?? "[NoFncName]";
        ReadOnlySpan<string?> args = ArgPtrsToStringSpan(argsPtr, argCount);

        StringBuilder output = new();
        output.Append("RvExtensionArgs Return String. Args:");
        output.Append(string.Join(", ", args.ToImmutableArray()));

        Interlocked.Increment(ref numberOfCalls);
        Encoding.UTF8.GetBytes(output.ToString()).CopyTo(outputBuffer.AsSpan());
        CallbackAfterDelay();
        return 0;
    }
    #endregion

    #region Singleton Fields
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);
    static ExtensionCallback? callback;
    static int numberOfCalls = 0;
    #endregion

    #region Unmanaged Helpers
    private static unsafe string? StringPtrToString(byte* ptr) =>
        Marshal.PtrToStringUTF8((IntPtr)ptr);

    private static unsafe ReadOnlySpan<string?> ArgPtrsToStringSpan(byte** argPtrs, int argCount) {
        string?[] args = new string[argCount];
        for (int i = 0; i < argCount; i++) {
            byte* argPtr = argPtrs[i];
            args[i] = Marshal.PtrToStringUTF8((IntPtr)argPtr);
        }
        return args;
    }
    #endregion

    #region Private Methods
    private static void CallbackAfterDelay() {
        int callNumber = Volatile.Read(ref numberOfCalls);
        Task.Run(async () => {
            await Task.Delay(TimeSpan.FromSeconds(1));
            callback?.Invoke(nameof(NativeAOTExtension), "RVCS_fnc_CallbackReceiver", $"Callback No: {callNumber:D}");
        });
    }
    #endregion
}
