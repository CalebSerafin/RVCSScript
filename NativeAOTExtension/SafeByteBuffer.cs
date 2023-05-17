using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NativeAOTExtension;
/// <summary>
/// Wrapps raw pointer so it can be used as an array or string.<br/>
/// NOT THREAD-SAFE, use external locking.<br/>
/// Calling dispose is not nessary for memory-leak management, but it ensures that the underlying resource will not be accessed.
/// </summary>
internal unsafe class SafeByteBuffer : IDisposable {
    public SafeByteBuffer(byte* bufferPtr, int bufferSize) {
        this.bufferPtr = bufferPtr;
        this.bufferSize = bufferSize;
    }

    #region Properties

    public byte this[int index] {
        get {
            AssertDispose();
            AssertIndexInRange(index);
            return bufferPtr[index];
        }
        set {
            AssertDispose();
            AssertIndexInRange(index);
            bufferPtr[index] = value;
        }
    }
    #endregion

    #region Public Methods
    public Span<byte> AsSpan() {
        AssertDispose();
        return AsSpan_Internal();
    }
    #endregion

    #region Fields
    readonly byte* bufferPtr;
    readonly int bufferSize;
    const byte NullChar = 0;
    #endregion

    #region Private Methods
    void AssertDispose() {
        if (disposedValue)
            throw new ObjectDisposedException(nameof(SafeByteBuffer), "The underlying resource MUST NOT be accessed to after the handle is finished execution.");
    }
    void AssertIndexInRange(int index) {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index was before first element");
        if (index >= bufferSize)
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index was after last element");
    }
    Span<byte> AsSpan_Internal() => new Span<byte>(bufferPtr, bufferSize);
    #endregion

    #region IDisposable
    private bool disposedValue;
    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                // TODO: dispose managed state (managed objects)
            }
            disposedValue = true;
        }
    }
    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
