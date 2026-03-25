using Xunit;
namespace Philiprehberger.AsyncLock.Tests;

public class AsyncOnceTests
{
    [Fact]
    public async Task GetAsync_ExecutesFactoryOnce()
    {
        var callCount = 0;
        using var once = new AsyncOnce<int>(async () =>
        {
            Interlocked.Increment(ref callCount);
            await Task.Delay(10);
            return 42;
        });

        var result1 = await once.GetAsync();
        var result2 = await once.GetAsync();

        Assert.Equal(42, result1);
        Assert.Equal(42, result2);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task IsCompleted_ReturnsFalseBeforeGet_TrueAfter()
    {
        using var once = new AsyncOnce<string>(() => Task.FromResult("hello"));

        Assert.False(once.IsCompleted);

        await once.GetAsync();

        Assert.True(once.IsCompleted);
    }

    [Fact]
    public void Constructor_NullFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new AsyncOnce<int>(null!));
    }

    [Fact]
    public async Task GetAsync_ConcurrentCalls_ExecutesFactoryOnce()
    {
        var callCount = 0;
        using var once = new AsyncOnce<int>(async () =>
        {
            Interlocked.Increment(ref callCount);
            await Task.Delay(50);
            return 99;
        });

        var tasks = Enumerable.Range(0, 10).Select(_ => once.GetAsync()).ToArray();
        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.Equal(99, r));
        Assert.Equal(1, callCount);
    }
}
