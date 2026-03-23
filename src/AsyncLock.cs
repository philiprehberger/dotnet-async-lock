namespace Philiprehberger.AsyncLock;

/// <summary>
/// Provides async-aware mutual exclusion using a <see cref="SemaphoreSlim"/> with a count of one.
/// Only one caller can hold the lock at a time.
/// </summary>
public sealed class AsyncLock : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Asynchronously acquires the lock. The returned <see cref="IDisposable"/> releases the lock when disposed.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the wait.</param>
    /// <returns>A <see cref="Releaser"/> that releases the lock when disposed.</returns>
    /// <example>
    /// <code>
    /// using (await asyncLock.LockAsync())
    /// {
    ///     // critical section
    /// }
    /// </code>
    /// </example>
    public async Task<Releaser> LockAsync(CancellationToken cancellationToken = default)
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
