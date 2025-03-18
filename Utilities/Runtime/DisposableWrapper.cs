using System;

namespace InfiniteCanvas.Utilities
{
	/// <summary>
	///     Wraps any object to add disposable behavior via a custom action
	///     Usage example: Add cleanup for non-IDisposable objects
	/// </summary>
	public class DisposableWrapper<T> : IDisposable
	{
		private readonly Action<T> _disposeAction;
		private readonly Trigger   _disposeTrigger;
		private readonly T         _instance;

		public DisposableWrapper(T instance, Action<T> disposeAction)
		{
			_instance = instance;
			_disposeAction = disposeAction;
			_disposeTrigger = new Trigger();
		}

		public void Dispose()
		{
			if (_disposeTrigger.Fire)
				_disposeAction(_instance);
		}
	}
}