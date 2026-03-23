# Philiprehberger.AsyncLock

[![CI](https://github.com/philiprehberger/dotnet-async-lock/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-async-lock/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.AsyncLock.svg)](https://www.nuget.org/packages/Philiprehberger.AsyncLock)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-async-lock)](LICENSE)

Async-aware synchronization primitives including AsyncLock, AsyncSemaphore, and AsyncOnce for safe async mutual exclusion.

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

- `LockAsync(CancellationToken cancellationToken = default)` — Acquires the lock asynchronously. Returns an `IDisposable` that releases the lock when disposed.

### `AsyncSemaphore`

- `AsyncSemaphore(int maxCount)` — Creates a semaphore with the specified maximum count.
- `WaitAsync(CancellationToken cancellationToken = default)` — Waits to enter the semaphore. Returns an `IDisposable` that releases the semaphore when disposed.
- `CurrentCount` — Gets the number of remaining entries.

### `AsyncOnce<T>`

- `AsyncOnce(Func<Task<T>> factory)` — Creates an instance with the specified async factory.
- `GetAsync(CancellationToken cancellationToken = default)` — Returns the cached value, executing the factory on first call.
- `IsCompleted` — Whether the factory has been executed.

### `Releaser`

- Readonly struct implementing `IDisposable` that releases a `SemaphoreSlim` on disposal.

## Development

```bash
dotnet build src/Philiprehberger.AsyncLock.csproj --configuration Release
```

## License

[MIT](LICENSE)
