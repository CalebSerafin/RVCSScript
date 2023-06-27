using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Logging;

namespace NativeAOTExtension.Logging;

/// <summary>
/// Safe wrapper for sending log messages via a native callback.
/// </summary>
internal class NativeLoggerProvider : ILoggerProvider
{
    /// <summary> Creates and empty logger provider, the native logger can be changed later. </summary>
    public NativeLoggerProvider() { }

    /// <summary> Creates logging provider that will forward log messages to the native callback. </summary>
    /// <param name="nativeLogPtr">Function poitner with arguments: int logLevel, int eventId, string eventName, string message</param>
    public NativeLoggerProvider(nint nativeLogPtr) : this() => SetNativeLogger(nativeLogPtr);

    /// <summary> Creates logging provider that will forward log messages to the native callback. </summary>
    /// <param name="nativeLog">Function poitner with arguments: int logLevel, int eventId, string eventName, string message</param>
    public NativeLoggerProvider(NativeLogDelegate nativeLog) : this() => SetNativeLogger(nativeLog);

    #region Properties
    public int LogLevel { get; set; } = (int)Microsoft.Extensions.Logging.LogLevel.Information;
    #endregion

    #region Public Methods
    public ILogger CreateLogger(string categoryName) =>
        new NativeLogger(RawLog, categoryName)
        {
            LogLevel = (LogLevel)LogLevel,
            GetLogLevel = () => (LogLevel)LogLevel,
        };

    public void SetNativeLogger(nint nativeLogPtr)
    {
        if (nativeLogPtr != nint.Zero)
            nativeLog = Marshal.GetDelegateForFunctionPointer<NativeLogDelegate>(nativeLogPtr);
    }

    public void SetNativeLogger(NativeLogDelegate? nativeLog) =>
        Volatile.Write(ref this.nativeLog, nativeLog);

    public void RawLog(int logLevel, int eventId, string eventName, string message)
    {
        if (disposedValue)
            return;
        Volatile.Read(ref nativeLog)?.Invoke(logLevel, eventId, eventName, message);
    }
    public void Log(StructuredLog message)
    {

    }
    #endregion

    #region Fields
    NativeLogDelegate? nativeLog;
    static readonly Lazy<JsonSerializerOptions> lazyJsonSerializerOptions = new(() => new JsonSerializerOptions()
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
        //TypeInfoResolver = TypeInfoRe


    });
    #endregion

    #region Private Methods
    void AssertDispose()
    {
        if (disposedValue)
            throw new ObjectDisposedException(nameof(NativeLogger), "The underlying resource MUST NOT be accessed to after the handle is finished execution.");
    }
    #endregion

    #region IDisposable
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~NativeCallbackLoggerProvider()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Type Definitions
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void NativeLogDelegate([MarshalAs(UnmanagedType.I4)] int logLevel, [MarshalAs(UnmanagedType.I4)] int eventId, [MarshalAs(UnmanagedType.LPStr)] string eventName, [MarshalAs(UnmanagedType.LPStr)] string message);
    #endregion
}
