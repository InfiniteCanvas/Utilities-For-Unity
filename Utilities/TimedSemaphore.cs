using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Utilities
{
	/// <summary>
	///     It's a SemaphoreSlim, but it releases automatically after a set time after calling WaitAsync.
	/// </summary>
	public class TimedSemaphore : IAsyncDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly TimeSpan                _releaseTime;
		private readonly SemaphoreSlim           _semaphore;

		/// <summary>
		///     It's a SemaphoreSlim, but it releases automatically after <paramref name="releaseTime" /> after calling WaitAsync.
		/// </summary>
		/// <param name="initialCount">Initial count, as with SemaphoreSlim</param>
		/// <param name="maximumCount">Maximum count, as with SemaphoreSlim</param>
		/// <param name="releaseTime">Time it takes for a handle to release</param>
		public TimedSemaphore(int initialCount, int maximumCount, TimeSpan releaseTime)
		{
			_releaseTime = releaseTime;
			_semaphore = new SemaphoreSlim(initialCount, maximumCount);
			_cancellationTokenSource = new CancellationTokenSource();
		}

		public async ValueTask DisposeAsync()
		{
			_cancellationTokenSource.Cancel();
			if (_semaphore               != null) await CastAndDispose(_semaphore);
			if (_cancellationTokenSource != null) await CastAndDispose(_cancellationTokenSource);

			return;

			static async ValueTask CastAndDispose(IDisposable resource)
			{
				if (resource is IAsyncDisposable resourceAsyncDisposable) await resourceAsyncDisposable.DisposeAsync();
				else resource.Dispose();
			}
		}

		public async UniTask WaitAsync(CancellationToken ct)
		{
			var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cancellationTokenSource.Token);
			await _semaphore.WaitAsync(cts.Token);
			UniTask.Delay(_releaseTime, true, PlayerLoopTiming.FixedUpdate, cts.Token)
			       .ContinueWith(Release)
			       .SuppressCancellationThrow()
			       .Forget();

			return;

			void Release()
			{
				_semaphore.Release();
				cts.Dispose();
			}
		}
	}
}