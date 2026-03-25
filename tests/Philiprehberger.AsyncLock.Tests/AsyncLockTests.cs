using Xunit;
namespace Philiprehberger.AsyncLock.Tests;

public class AsyncLockTests
{
    [Fact]
    public async Task LockAsync_AcquiresAndReleases_Successfully()
    {
        using var asyncLock = new AsyncLock();

        using (await asyncLock.LockAsync())
        {
            // Lock acquired — should not throw
        }

        // Lock released — should be able to acquire again
        using (await asyncLock.LockAsync())
        {
        }
    }

    [Fact]
    public async Task LockAsync_EnforcesMutualExclusion()
    {
        using var asyncLock = new AsyncLock();
        var counter = 0;
        var tasks = new List<Task>();

        for (var i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    var current = counter;
                    await Task.Yield();
                    counter = current + 1;
                }
            }));
        }

        await Task.WhenAll(tasks);

        Assert.Equal(100, counter);
    }

    [Fact]
    public async Task LockAsync_CancellationToken_ThrowsWhenCancelled()
    {
        using var asyncLock = new AsyncLock();
        using var cts = new CancellationTokenSource();

        // Hold the lock
        var releaser = await asyncLock.LockAsync();

        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => asyncLock.LockAsync(cts.Token));

        releaser.Dispose();
    }

    [Fact]
    public void Dispose_CanBeCalledSafely()
    {
        var asyncLock = new AsyncLock();
        asyncLock.Dispose();

        // Disposing again should not throw
        asyncLock.Dispose();
    }
}
