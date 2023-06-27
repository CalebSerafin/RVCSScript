using System.Buffers;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Extensions.Logging;
using NativeAOTExtension.Native;
using NativeAOTExtension.Projections;

namespace NativeAOTExtension;
public static class RVEntryPoint {
    #region RV Entry Points
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
        outputBuffer.OverwriteText("NativeAOTExtension v23.05.17");
        CallbackAfterDelay();
    }

    [UnmanagedCallersOnly(EntryPoint = "RVExtension", CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static unsafe void RvExtension(byte* outputPtr, int outputSize, byte* functionNamePtr) {
        using SafeByteBuffer outputBuffer = new(outputPtr, outputSize);
        string functionName = UnmanagedConverter.StringPtrToString(functionNamePtr) ?? "[NoFncName]";

        int callNumber = Interlocked.Increment(ref numberOfCalls);
        outputBuffer.OverwriteText($"function: {functionName}. This was call number {callNumber:D}.");
        CallbackAfterDelay();
    }

    [UnmanagedCallersOnly(EntryPoint = "RVExtensionArgs", CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static unsafe int RvExtensionArgs(byte* outputPtr, int outputSize, byte* functionNamePtr, byte** argsPtr, int argCount) {
        using SafeByteBuffer outputBuffer = new(outputPtr, outputSize);
        string functionName = UnmanagedConverter.StringPtrToString(functionNamePtr) ?? "[NoFncName]";
        ReadOnlySpan<string?> args = UnmanagedConverter.ArgPtrsToStringSpan(argsPtr, argCount);

        StringBuilder output = new();
        output.Append("RvExtensionArgs Return String. Args:");
        output.Append(string.Join(", ", args.ToImmutableArray()));

        Interlocked.Increment(ref numberOfCalls);
        outputBuffer.OverwriteText(output.ToString());
        string s = outputBuffer.ToString();
        CallbackAfterDelay();
        return 0;
    }
    #endregion

    #region Debug Entry Point
    public static void AttachLoggerCallback(ILogger logger) {
        RVEntryPoint.logger = logger;
    }
    #endregion

    #region Singleton Fields
    static ExtensionCallback? callback;
    static int numberOfCalls = 0;
    static ILogger? logger;
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

    #region Type Definitions
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);

    #endregion
}
