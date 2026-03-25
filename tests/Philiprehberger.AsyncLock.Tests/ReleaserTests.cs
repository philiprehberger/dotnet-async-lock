using Xunit;
namespace Philiprehberger.AsyncLock.Tests;

public class ReleaserTests
{
    [Fact]
    public void Constructor_NullSemaphore_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Releaser(null!));
    }

    [Fact]
    public void Dispose_ReleasesSemaphore()
    {
        var semaphore = new SemaphoreSlim(1, 1);
        semaphore.Wait();

        Assert.Equal(0, semaphore.CurrentCount);

        var releaser = new Releaser(semaphore);
        releaser.Dispose();

        Assert.Equal(1, semaphore.CurrentCount);
        semaphore.Dispose();
    }

    [Fact]
    public void Releaser_IsValueType()
    {
        Assert.True(typeof(Releaser).IsValueType);
    }
}
