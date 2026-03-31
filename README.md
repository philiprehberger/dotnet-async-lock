# Philiprehberger.AsyncLock

[![CI](https://github.com/philiprehberger/dotnet-async-lock/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-async-lock/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.AsyncLock.svg)](https://www.nuget.org/packages/Philiprehberger.AsyncLock)
[![Last updated](https://img.shields.io/github/last-commit/philiprehberger/dotnet-async-lock)](https://github.com/philiprehberger/dotnet-async-lock/commits/main)

Async-aware synchronization primitives — AsyncLock, AsyncSemaphore, and AsyncOnce for safe mutual exclusion.

## Installation

```bash
dotnet add package Philiprehberger.AsyncLock
```

## Usage

### AsyncLock

```csharp
using Philiprehberger.AsyncLock;

var asyncLock = new AsyncLock();

using (await asyncLock.LockAsync())
{
    // Critical section — only one caller at a time
    await DoWorkAsync();
}
```

### AsyncSemaphore

```csharp
var semaphore = new AsyncSemaphore(maxCount: 3);

using (await semaphore.WaitAsync())
{
    // Up to 3 concurrent callers allowed
    await DoWorkAsync();
}
```

### AsyncOnce

```csharp
var once = new AsyncOnce<HttpClient>(async () =>
{
    var client = new HttpClient();
    // expensive initialization
    return client;
});

var client = await once.GetAsync(); // Factory runs once, result is cached
```

## API

### `AsyncLock`

| Method | Description |
|--------|-------------|
| `LockAsync(CancellationToken cancellationToken = default)` | Acquires the lock asynchronously. Returns an `IDisposable` that releases the lock when disposed. |

### `AsyncSemaphore`

| Method | Description |
|--------|-------------|
| `AsyncSemaphore(int maxCount)` | Creates a semaphore with the specified maximum count. |
| `WaitAsync(CancellationToken cancellationToken = default)` | Waits to enter the semaphore. Returns an `IDisposable` that releases the semaphore when disposed. |
| `CurrentCount` | Gets the number of remaining entries. |

### `AsyncOnce<T>`

| Method | Description |
|--------|-------------|
| `AsyncOnce(Func<Task<T>> factory)` | Creates an instance with the specified async factory. |
| `GetAsync(CancellationToken cancellationToken = default)` | Returns the cached value, executing the factory on first call. |
| `IsCompleted` | Whether the factory has been executed. |

### `Releaser`

| Method | Description |
|--------|-------------|
| `Releaser` | Readonly struct implementing `IDisposable` that releases a `SemaphoreSlim` on disposal. |

## Development

```bash
dotnet build src/Philiprehberger.AsyncLock.csproj --configuration Release
```

## Support

If you find this project useful:

⭐ [Star the repo](https://github.com/philiprehberger/dotnet-async-lock)

🐛 [Report issues](https://github.com/philiprehberger/dotnet-async-lock/issues?q=is%3Aissue+is%3Aopen+label%3Abug)

💡 [Suggest features](https://github.com/philiprehberger/dotnet-async-lock/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

❤️ [Sponsor development](https://github.com/sponsors/philiprehberger)

🌐 [All Open Source Projects](https://philiprehberger.com/open-source-packages)

💻 [GitHub Profile](https://github.com/philiprehberger)

🔗 [LinkedIn Profile](https://www.linkedin.com/in/philiprehberger)

## License

[MIT](LICENSE)
