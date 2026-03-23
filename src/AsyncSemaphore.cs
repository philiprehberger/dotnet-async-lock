namespace Philiprehberger.AsyncLock;

/// <summary>
/// An async-aware counting semaphore that allows a configurable number of concurrent callers.
/// </summary>
public sealed class AsyncSemaphore : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSemaphore"/> class.
    /// </summary>
    /// <param name="maxCount">The maximum number of concurrent entries allowed.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxCount"/> is less than 1.</exception>
    public AsyncSemaphore(int maxCount)
    {
        if (maxCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxCount), maxCount, "Maximum count must be at least 1.");
        }

        _semaphore = new SemaphoreSlim(maxCount, maxCount);
    }

    /// <summary>
    /// Gets the number of remaining entries that can be granted concurrently.
    /// </summary>
    public int CurrentCount => _semaphore.CurrentCount;

    /// <summary>
    /// Asynchronously waits to enter the semaphore. The returned <see cref="IDisposable"/>
    /// releases the semaphore entry when disposed.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the wait.</param>
    /// <returns>A <see cref="Releaser"/> that releases the semaphore when disposed.</returns>
    public async Task<Releaser> WaitAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new Releaser(_semaphore);
    }

    /// <summary>
    /// Releases the underlying semaphore resources.
    /// </summary>
    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
