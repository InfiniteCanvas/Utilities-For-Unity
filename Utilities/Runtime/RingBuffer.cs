using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace InfiniteCanvas.Utilities
{
    /// <summary>
    ///     Basically a <see cref="Queue{T}" /> but O(1)!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RingBuffer<T> : IEnumerable<T>
    {
        private T[] _buffer;

        private int _head;

        // I dislike throwing, but I still put it in just in case
        private bool _returnDefault;
        private int  _tail;

        /// <summary>
        ///     Constructor. The capacity provided is rounded up to the next power of 2.
        /// </summary>
        /// <param name="capacity">
        ///     Initial capacity of the buffer. If not a power of 2, will be set to the closest bigger power of
        ///     2.
        /// </param>
        public RingBuffer(int capacity, bool returnDefault = true)
        {
            _returnDefault = returnDefault;
            _buffer = new T[NextPowerOf2(capacity)];
        }

        public int Count { get; private set; }

        /// <summary>
        ///     Indexer for accessing buffer elements.
        /// </summary>
        public T this[int index]
        {
            get
            {
                if (ValidIndex(index)) return _buffer[WrapIndex(_head + index)];

                if (_returnDefault) return default;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

    #region IEnumerable<T> Members

        /// <summary>
        ///     Returns an enumerator to iterate through the buffer.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            if (_head < _tail)
            {
                for (var i = _head; i < _tail; i++)
                    yield return _buffer[i];
            }
            else // need to wrap
            {
                for (var i = _head; i < _buffer.Length; i++)
                    yield return _buffer[i];
                for (var i = 0; i < _tail; i++)
                    yield return _buffer[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

        /// <summary>
        ///     Computes the next power of 2 greater than or equal to x.
        /// </summary>
        private static int NextPowerOf2(int x)
        {
            if (x < 1)
                return 1;

            // in case we are already at a power of 2
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            // we are done because we're using int32 here in c#
            x++;
            return x;
        }

        /// <summary>
        ///     Adds an item to the end of the buffer.
        /// </summary>
        public void Enqueue(T item)
        {
            if (Count == _buffer.Length)
                Resize();

            _buffer[_tail] = item;
            _tail = WrapIndex(_tail + 1);
            Count++;
        }

        /// <summary>
        ///     Removes and returns the item at the head of the buffer.
        /// </summary>
        public T Dequeue()
        {
            if (Count == 0)
            {
                if (_returnDefault) return default;
                throw new InvalidOperationException("Buffer is empty");
            }

            var item = _buffer[_head];
            _buffer[_head] = default;
            _head = WrapIndex(_head + 1);
            Count--;
            return item;
        }

        /// <summary>
        ///     Returns the item at the head of the buffer without removing it.
        /// </summary>
        public T Peek()
        {
            if (Count != 0) return _buffer[_head];

            if (_returnDefault) return default;
            throw new InvalidOperationException("Buffer is empty");
        }

        /// <summary>
        ///     Returns whether the buffer is empty.
        /// </summary>
        public bool IsEmpty() => Count == 0;

        /// <summary>
        ///     Clears the contents of the buffer.
        /// </summary>
        /// <remarks>
        ///     Doesn't actually clear the buffer unless <see cref="emptyBuffer" /> is true, then it sets each buffer element to
        ///     default(<see cref="T" />)
        /// </remarks>
        public void Clear(bool emptyBuffer = false)
        {
            _head = _tail = Count = 0;
            if (!emptyBuffer) return;
            for (var i = 0; i < _buffer.Length; i++) _buffer[i] = default;
        }

        /// <summary>
        ///     Resizes the buffer, doubling its current size.
        /// </summary>
        private void Resize()
        {
            var resizedBuffer = new T[Count << 1];

            if (_head < _tail)
            {
                Array.Copy(_buffer, _head, resizedBuffer, 0, Count);
            }
            else // need to wrap
            {
                var elementsFirstPart = Count - _head;
                Array.Copy(_buffer, _head, resizedBuffer, 0,                 elementsFirstPart);
                Array.Copy(_buffer, 0,     resizedBuffer, elementsFirstPart, _tail);
            }

            _buffer = resizedBuffer;
            _head = 0;
            _tail = Count;
        }

        /// <summary>
        ///     Validates that the given index is within the bounds of the current count.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ValidIndex(int index) => index >= 0 && index < Count;

        /// <summary>
        ///     Efficiently wraps an index using bitwise AND (this is why _buffer.Length needs to be a power of 2).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int WrapIndex(int index) => index & (_buffer.Length - 1);
    }
}