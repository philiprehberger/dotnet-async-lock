namespace Philiprehberger.AsyncLock;

/// <summary>
/// Executes an async factory exactly once and caches the result.
/// Thread-safe using <see cref="SemaphoreSlim"/> for async synchronization.
/// </summary>
/// <typeparam name="T">The type of value produced by the factory.</typeparam>
public sealed class AsyncOnce<T> : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly Func<Task<T>> _factory;
    private T? _value;
    private bool _isCompleted;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOnce{T}"/> class.
    /// </summary>
    /// <param name="factory">The async factory function to execute once.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is null.</exception>
    public AsyncOnce(Func<Task<T>> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// Gets a value indicating whether the factory has been executed and the value is cached.
    /// </summary>
    public bool IsCompleted => _isCompleted;

    /// <summary>
    /// Gets the cached value, executing the factory on the first call.
    /// Subsequent calls return the cached value without re-executing the factory.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the wait if the factory has not yet completed.</param>
    /// <returns>The cached value produced by the factory.</returns>
    public async Task<T> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_isCompleted)
        {
            return _value!;
        }

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!_isCompleted)
            {
                _value = await _factory().ConfigureAwait(false);
                _isCompleted = true;
            }

            return _value!;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Releases the underlying semaphore resources.
    /// </summary>
    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
