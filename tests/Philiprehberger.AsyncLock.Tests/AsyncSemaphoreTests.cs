using Xunit;
namespace Philiprehberger.AsyncLock.Tests;

public class AsyncSemaphoreTests
{
    [Fact]
    public async Task WaitAsync_AcquiresAndReleases_Successfully()
    {
        using var semaphore = new AsyncSemaphore(2);

        Assert.Equal(2, semaphore.CurrentCount);

        using (await semaphore.WaitAsync())
        {
            Assert.Equal(1, semaphore.CurrentCount);
        }

        Assert.Equal(2, semaphore.CurrentCount);
    }

    [Fact]
    public async Task WaitAsync_RespectsMaxCount()
    {
        using var semaphore = new AsyncSemaphore(2);
        var entered = 0;
        var maxConcurrent = 0;
        var lockObj = new object();

        var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(async () =>
        {
            using (await semaphore.WaitAsync())
            {
                lock (lockObj)
                {
                    entered++;
                    if (entered > maxConcurrent) maxConcurrent = entered;
                }

                await Task.Delay(20);

                lock (lockObj) entered--;
            }
        })).ToArray();

        await Task.WhenAll(tasks);

        Assert.True(maxConcurrent <= 2);
    }

    [Fact]
    public void Constructor_ZeroMaxCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncSemaphore(0));
    }

    [Fact]
    public void Constructor_NegativeMaxCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncSemaphore(-1));
    }

    [Fact]
    public async Task WaitAsync_CancellationToken_ThrowsWhenCancelled()
    {
        using var semaphore = new AsyncSemaphore(1);
        using var cts = new CancellationTokenSource();

        // Exhaust the semaphore
        var releaser = await semaphore.WaitAsync();

        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => semaphore.WaitAsync(cts.Token));

        releaser.Dispose();
    }
}
