using Xunit.Extensions.Ordering;

namespace Kulaszka.Collections.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task EnqueueTwoElements()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            Assert.Equal(2, queue.Count);
        }


        [Fact]
        public async Task EnqueueThirdElement_throws_TimeoutException_after_Waiting500Milliceconds()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            await Assert.ThrowsAsync<TimeoutException>(async () => await queue.EnqueueAsync("c", 5, 500));
        }

        [Fact]
        public async Task EnqueueThirdElement_throws_TimeoutException_after_Waiting2Second()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            await Assert.ThrowsAsync<TimeoutException>(async () => await queue.EnqueueAsync("c", 5, TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task EnqueueThirdElement_throws_OperationCancelledException()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            var tcs = new CancellationTokenSource();

            var assertTask = Assert.ThrowsAsync<OperationCanceledException>(async () => await queue.EnqueueAsync("c", 5, TimeSpan.FromSeconds(5), tcs.Token));

            tcs.CancelAfter(TimeSpan.FromSeconds(1));

            await assertTask;
        }

        [Fact]
        public async Task EnqueueThirdElementToClosedQueue_throws_QueueIsClosedException()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            queue.CompleteAdding();

            await Assert.ThrowsAsync<QueueIsClosedException>(async () => await queue.EnqueueAsync("c", 5, TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task Queue_is_empty_after_dequeue_all_two_elements()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            await queue.DequeueAsync();
            await queue.DequeueAsync();

            Assert.Equal(0, queue.Count);
        }

        [Fact]
        public async Task DequeueElement_from_empty_Queue_throws_TimeoutException_after_Waiting500Milliceconds()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            await queue.DequeueAsync();
            await queue.DequeueAsync();

            await Assert.ThrowsAsync<TimeoutException>(async () => await queue.DequeueAsync(500));
        }

        [Fact]
        public async Task DequeueElement_from_empty_Queue_throws_TimeoutException_after_Waiting1Second()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            await queue.DequeueAsync();
            await queue.DequeueAsync();

            await Assert.ThrowsAsync<TimeoutException>(async () => await queue.DequeueAsync(TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task DequeueElement_throws_OperationCancelledException()
        {
            using var queue = new BlockingPriorityQueue<string, int>(2);

            await queue.EnqueueAsync("a", 5);
            await queue.EnqueueAsync("b", 5);

            await queue.DequeueAsync();
            await queue.DequeueAsync();

            var tcs = new CancellationTokenSource();

            var assertTask = Assert.ThrowsAsync<OperationCanceledException>(async () => await queue.DequeueAsync(TimeSpan.FromSeconds(5), tcs.Token));

            tcs.CancelAfter(TimeSpan.FromSeconds(1));

            await assertTask;
        }
    }
}