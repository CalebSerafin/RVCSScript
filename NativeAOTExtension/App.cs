using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NativeAOTExtension;
internal class App : IDisposable, IAsyncDisposable {
    public App() {
        HostApplicationBuilderSettings settings = new() {
            ApplicationName = "NativeAOTExtension",
        };
        HostApplicationBuilder builder = new(settings);
        // Todo, configure services.
        _ = builder.Services;
        Host = builder.Build();
        // ToDo, configure app.
        _ = Host.Services;
        Host.StartAsync();
    }

    #region Properties
    public IHost Host { get; private init; }
    #endregion

    #region IDisposable & IAsyncDisposable
    private bool disposedValue;
    protected virtual async ValueTask DisposeAsync(bool disposing) {
        if (disposedValue)
            return;
        if (disposing) {
            // TODO: dispose managed state (managed objects)
            await Host.StopAsync(TimeSpan.FromSeconds(1));
            Host.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
    }

    public void Dispose() {
        DisposeAsync();
    }

    public ValueTask DisposeAsync() {
        GC.SuppressFinalize(this);
        return DisposeAsync(disposing: true);
    }
    #endregion
}
