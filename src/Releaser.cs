namespace Philiprehberger.AsyncLock;

/// <summary>
/// A lightweight disposable struct that releases a <see cref="SemaphoreSlim"/> when disposed.
/// </summary>
public readonly struct Releaser : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// Initializes a new instance of the <see cref="Releaser"/> struct.
    /// </summary>
    /// <param name="semaphore">The semaphore to release on disposal.</param>
    public Releaser(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore));
    }

    /// <summary>
    /// Releases the semaphore.
    /// </summary>
    public void Dispose()
    {
        _semaphore.Release();
    }
}
