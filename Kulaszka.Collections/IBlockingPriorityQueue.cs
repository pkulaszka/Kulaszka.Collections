
namespace Kulaszka.Collections
{
    /// <summary>
    /// Blocking and priority queue interface
    /// </summary>
    /// <typeparam name="TElement">Value type for queue items.</typeparam>
    /// <typeparam name="TPriority">Value type for priorities.</typeparam>
    public interface IBlockingPriorityQueue<TElement, TPriority>
        where TElement : notnull
        where TPriority : notnull
    {
        /// <summary>
        /// Number of items in the queue.
        /// </summary>
        long Count { get; }

        /// <summary>
        /// Indicates whether new items can be added to the queue.
        /// </summary>
        bool IsAddingCompleted { get; }

        /// <summary>
        /// Stop handling new items.
        /// </summary>
        void CompleteAdding();

        /// <summary>
        /// Inserts a new element to the queue even if the queue is full (no waits until a place in the queue becomes available).
        /// </summary>
        /// <param name="element">The object being added to the queue.</param>
        /// <param name="priority">The priority assigned to the object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        void ForceEnqueue(TElement element, TPriority priority, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a new element to the queue. If the queue is full, waits until a place in the queue becomes available or a cancellation signal is received or timeout occurs.
        /// </summary>
        /// <param name="element">The object being added to the queue.</param>
        /// <param name="priority">The priority assigned to the object.</param>
        /// <param name="millisecondsTimeout">The time after which waiting for place in the queue will end by throwing an TimeoutException exception.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that will be completed when the item is added to the queue.</returns>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the element or priority is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is canceled.</exception>
        /// <exception cref="TimeoutException">Thrown when the timeout expires.</exception>
        Task EnqueueAsync(TElement element, TPriority priority, int millisecondsTimeout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a new element to the queue. If the queue is full, waits until a place in the queue becomes available or a cancellation signal is received or timeout occurs.
        /// </summary>
        /// <param name="element">The object being added to the queue.</param>
        /// <param name="priority">The priority assigned to the object.</param>
        /// <param name="timeout">The time after which waiting for the place in the queue will end by throwing an TimeoutException exception.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that will be completed when the item is added to the queue.</returns>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the element or priority is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is canceled.</exception>
        /// <exception cref="TimeoutException">Thrown when the timeout expires.</exception>
        Task EnqueueAsync(TElement element, TPriority priority, TimeSpan timeout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a new element to the queue. If the queue is full, waits until a place in the queue becomes available or a cancellation signal is received.
        /// </summary>
        /// <param name="element">The object being added to the queue.</param>
        /// <param name="priority">The priority assigned to the object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that will be completed when the item is added to the queue.</returns>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the element or priority is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is canceled.</exception>
        /// <exception cref="TimeoutException">Thrown when the timeout expires.</exception>
        Task EnqueueAsync(TElement element, TPriority priority, CancellationToken cancellationToken = default);

        /// <summary>
        /// Takes a next element from the queue. If the queue is empty, waits until a new element is inserted to the queue or a cancellation signal is received.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>TElement from the queue.</returns>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is canceled.</exception>
        Task<TElement> DequeueAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Takes a next element from the queue. If the queue is empty, waits until a new element is inserted to the queue or a cancellation signal is received or timeout occurs.
        /// </summary>
        /// <param name="millisecondsTimeout">The time after which waiting for the place in the queue will end by throwing an TimeoutException exception.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>TElement from the queue.</returns>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is canceled.</exception>
        /// <exception cref="TimeoutException">Thrown when the timeout expires.</exception>
        Task<TElement> DequeueAsync(int millisecondsTimeout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Takes a next element from the queue. If the queue is empty, waits until a new element is inserted to the queue or a cancellation signal is received or timeout occurs.
        /// </summary>
        /// <param name="timeout">The time after which waiting for the place in the queue will end by throwing an TimeoutException exception.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>TElement from the queue.</returns>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is canceled.</exception>
        /// <exception cref="TimeoutException">Thrown when the timeout expires.</exception>
        Task<TElement> DequeueAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}
