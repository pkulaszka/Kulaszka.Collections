
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
        /// Inserts a new element to the queue. If the queue is full, waits until a place in the queue becomes available.
        /// </summary>
        /// <param name="element">The object being added to the queue.</param>
        /// <param name="priority">The priority assigned to the object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that will be completed when the item is added to the queue.</returns>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the element or priority is null.</exception>
        Task EnqueueAsync(TElement element, TPriority priority, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a new element to the queue. If the queue is full, waits until a place in the queue becomes available.
        /// </summary>
        /// <param name="element">The object being added to the queue.</param>
        /// <param name="priority">The priority assigned to the object.</param>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that will be completed when the item is added to the queue.</returns>
        /// <exception cref="QueueIsClosedException">Thrown when the queue is closed and no more items can be added.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the queue is disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the element or priority is null.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        Task EnqueueAsync(TElement element, TPriority priority, int millisecondsTimeout = Timeout.Infinite, CancellationToken cancellationToken = default);

        /// <summary>
        /// Takes a next element from the queue. If the queue is empty, waits until a new element is inserted to the queue.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<TElement> DequeueAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a new element to the queue even if the queue is full (no waits until a place in the queue becomes available).
        /// </summary>
        /// <param name="element">The object being added to the queue.</param>
        /// <param name="priority">The priority assigned to the object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        void ForceEnqueue(TElement element, TPriority priority, CancellationToken cancellationToken = default);

        void CompleteAdding();
    }
}
