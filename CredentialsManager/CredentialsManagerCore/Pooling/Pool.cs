using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Serilog;

namespace Draeger.Testautomation.CredentialsManagerCore.Pooling
{
    public class Pool<T> : IDisposable
    {
        private readonly Func<Pool<T>, T> _factory;
        private readonly LoadingMode _loadingMode;
        private readonly IItemStore _itemStore;
        private readonly int _size;
        private int _count;
        private readonly Semaphore _sync;

        public Pool(int size, Func<Pool<T>, T> factory)
            : this(size, factory, LoadingMode.Lazy, AccessMode.Fifo)
        {
        }

        public Pool(int size, Func<Pool<T>, T> factory,
            LoadingMode loadingMode, AccessMode accessMode)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size", size,
                    "Argument 'size' must be greater than zero.");
            this._size = size;
            this._factory = factory ?? throw new ArgumentNullException("factory");
            _sync = new Semaphore(size, size);
            this._loadingMode = loadingMode;
            this._itemStore = CreateItemStore(accessMode, size);
            if (loadingMode == LoadingMode.Eager)
            {
                PreloadItems();
            }
        }

        public Type GetPooledType()
        {
            return typeof(T);
        }

        public T Acquire(ILogger logger)
        {
            logger.Debug("Acquire start");
            _sync.WaitOne();
            logger.Debug("Acquire WaitOne");
            switch (_loadingMode)
            {
                case LoadingMode.Eager:
                    logger.Debug("Acquire Eager");
                    return AcquireEager();
                case LoadingMode.Lazy:
                    logger.Debug("Acquire Lazy");
                    return AcquireLazy();
                default:
                    logger.Debug("Acquire AcquireLazyExpanding");
                    Debug.Assert(_loadingMode == LoadingMode.LazyExpanding,
                        "Unknown LoadingMode encountered in Acquire method.");
                    return AcquireLazyExpanding();
            }
        }

        public void Release(T item)
        {
            lock (_itemStore)
            {
                _itemStore.Store(item);
            }
            _sync.Release();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;
            if (!disposing) return;
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                lock (_itemStore)
                {
                    while (_itemStore.Count > 0)
                    {
                        var disposable = (IDisposable)_itemStore.Fetch();
                        disposable.Dispose();
                    }
                }
            }
            _sync.Close();
            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #region Acquisition

        private T AcquireEager()
        {
            lock (_itemStore)
            {
                return _itemStore.Fetch();
            }
        }

        private T AcquireLazy()
        {
            lock (_itemStore)
            {
                if (_itemStore.Count > 0)
                {
                    return _itemStore.Fetch();
                }
            }
            Interlocked.Increment(ref _count);
            return _factory(this);
        }

        private T AcquireLazyExpanding()
        {
            var shouldExpand = false;
            if (_count < _size)
            {
                var newCount = Interlocked.Increment(ref _count);
                if (newCount <= _size)
                {
                    shouldExpand = true;
                }
                else
                {
                    // Another thread took the last spot - use the store instead
                    Interlocked.Decrement(ref _count);
                }
            }
            if (shouldExpand)
            {
                return _factory(this);
            }
            else
            {
                lock (_itemStore)
                {
                    return _itemStore.Fetch();
                }
            }
        }

        private void PreloadItems()
        {
            for (var i = 0; i < _size; i++)
            {
                var item = _factory(this);
                _itemStore.Store(item);
            }
            _count = _size;
        }

        #endregion

        #region Collection Wrappers

        interface IItemStore
        {
            T Fetch();
            void Store(T item);
            int Count { get; }
        }

        private IItemStore CreateItemStore(AccessMode mode, int capacity)
        {
            switch (mode)
            {
                case AccessMode.Fifo:
                    return new QueueStore(capacity);
                case AccessMode.Lifo:
                    return new StackStore(capacity);
                default:
                    Debug.Assert(mode == AccessMode.Circular,
                        "Invalid AccessMode in CreateItemStore");
                    return new CircularStore(capacity);
            }
        }

        class QueueStore : Queue<T>, IItemStore
        {
            public QueueStore(int capacity) : base(capacity)
            {
            }

            public T Fetch()
            {
                return Dequeue();
            }

            public void Store(T item)
            {
                Enqueue(item);
            }
        }

        class StackStore : Stack<T>, IItemStore
        {
            public StackStore(int capacity) : base(capacity)
            {
            }

            public T Fetch()
            {
                return Pop();
            }

            public void Store(T item)
            {
                Push(item);
            }
        }

        class CircularStore : IItemStore
        {
            private readonly List<Slot> _slots;
            private int _position = -1;

            public CircularStore(int capacity)
            {
                _slots = new List<Slot>(capacity);
                Count = capacity;
            }

            public T Fetch()
            {
                if (Count == 0)
                    throw new InvalidOperationException("The buffer is empty.");

                var startPosition = _position;
                do
                {
                    Advance();
                    var slot = _slots[_position];
                    if (!slot.IsInUse)
                    {
                        slot.IsInUse = true;
                        --Count;
                        return slot.Item;
                    }
                } while (startPosition != _position);
                throw new InvalidOperationException("No free slots.");
            }

            public void Store(T item)
            {
                var slot = _slots.Find(s => Equals(s.Item, item));
                if (slot == null)
                {
                    slot = new Slot(item);
                    _slots.Add(slot);
                }
                slot.IsInUse = false;
                ++Count;
            }

            public int Count { get; private set; }

            private void Advance()
            {
                _position = (_position + 1) % _slots.Count;
            }

            class Slot
            {
                public Slot(T item)
                {
                    this.Item = item;
                }

                public T Item { get; private set; }
                public bool IsInUse { get; set; }
            }
        }

        #endregion

        public bool IsDisposed { get; private set; }
    }
}