using System.Collections.Concurrent;
using System.Threading;

namespace Kulaszka.Collections
{
    /// <summary>
    /// Blocking and priority queue implementation.
    /// </summary>
    /// <typeparam name="TElement">Value type for queue items.</typeparam>
    /// <typeparam name="TPriority">Value type for priorities.</typeparam>
    public class BlockingPriorityQueue<TElement, TPriority> : IBlockingPriorityQueue<TElement, TPriority>, IDisposable
        where TElement : notnull
        where TPriority : notnull
    {
        private readonly ConcurrentDictionary<TPriority, ConcurrentQueue<TElement>> _queues;
        private readonly AutoResetEvent _allowEnqueue;
        private readonly AutoResetEvent _allowDequeue;

        private readonly ReaderWriterLockSlim _countLock;
        private readonly ReaderWriterLockSlim _completeAddingLock;

        private readonly long _boundedCapacity;
        private long _count;
        private bool _completeAdding;
        private bool disposedValue;

        public BlockingPriorityQueue(int boundedCapacity)
        {
            _countLock = new ReaderWriterLockSlim();
            _completeAddingLock = new ReaderWriterLockSlim();

            _completeAdding = false;
            _boundedCapacity = boundedCapacity;
            _count = 0;
            _queues = new ConcurrentDictionary<TPriority, ConcurrentQueue<TElement>>();

            _allowEnqueue = new AutoResetEvent(_count < _boundedCapacity);
            _allowDequeue = new AutoResetEvent(_count > 0);
        }

        public long Count
        {
            get
            {
                _countLock.EnterReadLock();
                try
                {
                    return _count;
                }
                finally
                {
                    _countLock.ExitReadLock();
                }
            }
        }

        public bool IsAddingCompleted
        {
            get
            {
                _completeAddingLock.EnterReadLock();
                try
                {
                    return _completeAdding;
                }
                finally
                {
                    _completeAddingLock.ExitReadLock();
                }
            }
            private set
            {
                _completeAddingLock.EnterWriteLock();
                try
                {
                    _completeAdding = value;
                }
                finally
                {
                    _completeAddingLock.ExitWriteLock();
                }
            }
        }

        public void CompleteAdding()
        {
            IsAddingCompleted = true;
        }

        public void ForceEnqueue(TElement element, TPriority priority, CancellationToken cancellationToken = default)
        {
            ValidateBeforeEnqueue(element, priority);

            cancellationToken.ThrowIfCancellationRequested();

            Enqueue(element, priority);
        }

        public async Task EnqueueAsync(TElement element, TPriority priority, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            ValidateBeforeEnqueue(element, priority);

            var canEnqueue = await _allowEnqueue.WaitOneAsync(millisecondsTimeout, cancellationToken); ;

            cancellationToken.ThrowIfCancellationRequested();

            if (!canEnqueue)
            {
                throw new TimeoutException();
            }

            if (IsAddingCompleted)
            {
                throw new QueueIsClosedException();
            }

            Enqueue(element, priority);
        }

        public async Task EnqueueAsync(TElement element, TPriority priority, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            await EnqueueAsync(element, priority, (int)timeout.TotalMilliseconds, cancellationToken);
        }

        public async Task EnqueueAsync(TElement element, TPriority priority, CancellationToken cancellationToken = default)
        {
            await EnqueueAsync(element, priority, Timeout.Infinite, cancellationToken);
        }

        private void ValidateBeforeEnqueue(TElement element, TPriority priority)
        {
            if (element == null)
            {
                throw new InvalidOperationException($"{nameof(element)} cannot be null.", new ArgumentNullException(nameof(element)));
            }

            if (priority == null)
            {
                throw new InvalidOperationException($"{nameof(priority)} cannot be null.", new ArgumentNullException(nameof(priority)));
            }

            if (IsAddingCompleted)
            {
                throw new QueueIsClosedException();
            }
        }

        private void Enqueue(TElement element, TPriority priority)
        {
            if (!_queues.ContainsKey(priority))
            {
                lock (_queues)
                {
                    if (!_queues.ContainsKey(priority))
                    {
                        _queues.TryAdd(priority, new ConcurrentQueue<TElement>());
                    }
                }
            }

            var queue = _queues[priority];

            queue.Enqueue(element);

            UpdateCount(1);
        }

        public async Task<TElement> DequeueAsync(int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            var canDequeue = await _allowDequeue.WaitOneAsync(millisecondsTimeout, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            
            if (!canDequeue)
            {
                throw new TimeoutException();
            }
            

            IList<TPriority> orderedKeys;
            lock (_queues)
            {
                orderedKeys = _queues.Keys.OrderBy(x => x).ToList();
            }

            foreach (var priority in orderedKeys)
            {
                var queue = _queues[priority];
                if (queue.Count > 0 && queue.TryDequeue(out var element))
                {
                    UpdateCount(-1);

                    return element;
                }
            }

            throw new InvalidOperationException($"Unrecognized queue state!");
        }
        public async Task<TElement> DequeueAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            return await DequeueAsync((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public async Task<TElement> DequeueAsync(CancellationToken cancellationToken = default)
        {
            return await DequeueAsync(Timeout.Infinite, cancellationToken);
        }

        private void UpdateCount(long value)
        {
            _countLock.EnterWriteLock();
            try
            {
                long count = Interlocked.Add(ref _count, value);

                if (count > 0)
                {
                    _allowDequeue.Set();
                }

                if (count < _boundedCapacity)
                {
                    _allowEnqueue.Set();
                }
            }
            finally
            {
                _countLock.ExitWriteLock();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CompleteAdding();

                    lock(_queues)
                    {
                        foreach(var queue in _queues.Values)
                        {
                            queue.Clear();
                        }

                        _queues.Clear();
                    }

                    _countLock.Dispose();
                    _completeAddingLock.Dispose();

                    _allowEnqueue.Dispose();
                    _allowDequeue.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WorkQueue()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
