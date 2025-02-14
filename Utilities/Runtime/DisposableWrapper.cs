using System;

namespace InfiniteCanvas.Utilities
{
    public class DisposableWrapper<T> : IDisposable
    {
        private readonly Action<T> _disposeAction;
        private readonly T         _instance;

        public DisposableWrapper(T instance, Action<T> disposeAction)
        {
            _instance = instance;
            _disposeAction = disposeAction;
        }

        public void Dispose() => _disposeAction(_instance);
    }
}