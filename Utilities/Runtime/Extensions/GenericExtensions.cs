using System;

namespace InfiniteCanvas.Utilities.Extensions
{
    public static class GenericExtensions
    {
        public static DisposableWrapper<T> CreateDisposableWrapper<T>(this T obj, Action<T> action) where T : new()
            => new(obj, action);
    }
}