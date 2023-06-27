using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NativeAOTExtension.Logging;
internal class NativeLogger : ILogger
{
    #region Constructor
    public NativeLogger(RawLogDelegate rawLogDelegate, string? name)
    {
        this.rawLogDelegate = rawLogDelegate;
        this.name = name;
    }
    protected NativeLogger() { }
    #endregion

    #region Properties
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public Func<LogLevel>? GetLogLevel { get; set; }
    public readonly static NativeLogger VoidLogger = new();
    #endregion

    #region ILogger
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return new LoggingScope<TState>(state);
    }
    public bool IsEnabled(LogLevel logLevel)
    {
        LogLevel enabledLebel = GetLogLevel?.Invoke() ?? LogLevel;
        return logLevel >= enabledLebel;
    }
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (rawLogDelegate is null || !IsEnabled(logLevel))
            return;
        string message = FormatLogMessage(formatter(state, exception), exception);
        rawLogDelegate((int)logLevel, eventId.Id, eventId.Name ?? eventId.Id.ToString(), message);
    }
    #endregion

    #region Fields
    protected readonly string? name;
    readonly RawLogDelegate? rawLogDelegate;
    #endregion

    #region Private Methods
    protected virtual string FormatLogMessage(string? embeddedMessage, Exception? exception)
    {
        string? exceptionMessage = exception?.ToString();
        int allocatedSize = (name?.Length ?? 0) + (embeddedMessage?.Length ?? 0) + (exceptionMessage?.Length ?? 0);
        StringBuilder builder = new(allocatedSize);

        if (name is not null) builder.Append($"Logger: {name}; ");
        if (embeddedMessage is not null) builder.Append($"Message: {embeddedMessage};");
        if (exception is not null) builder.Append($"Exception: {exceptionMessage};");

        return builder.ToString();
    }
    #endregion

    #region Type Definitions
    public delegate void RawLogDelegate(int logLevel, int eventId, string eventName, string message);
    record class LoggingScope<TState>(TState State) : IDisposable
    {
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
        // ~LoggingScope()
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
    }
    #endregion
}

internal class NativeCallbackLogger<TCategoryName> : NativeLogger, ILogger<TCategoryName>
{
    protected override string FormatLogMessage(string? embeddedMessage, Exception? exception)
    {
        string? exceptionMessage = exception?.ToString();
        StringBuilder builder = new();

        builder.Append("{ ");
        if (name is not null) builder.Append($"Logger: {name}, ");
        if (name is not null) builder.Append($"Category: {typeof(TCategoryName).FullName}, ");
        if (embeddedMessage is not null) builder.Append($"Message: {embeddedMessage},");
        if (exception is not null) builder.Append($"Exception: {exceptionMessage},");
        builder.Append(" }");

        return builder.ToString();
    }
}

internal record class StructuredLog(string? Logger, string? Category, string? Message, string? Exception);

internal static class NativeLogger_Extensions
{
    public static IServiceCollection AddNativeCallbackLogger(this IServiceCollection serviceCollection) => serviceCollection
        .AddSingleton<ILogger, NativeLogger>()
        .AddSingleton<ILoggerProvider, NativeLoggerProvider>();
}
