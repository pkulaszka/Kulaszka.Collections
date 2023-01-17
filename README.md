# Kulaszka.Collections

Example usage:

using(var queue = new BlockingPriorityQueue<string, int>(2))<br>
{<br>
  // capacity = 2, available = 2, count = 0<br>
  
  // adds element to the queue. Capacity = 2, available = 1, count = 1<br>
  await queue.EnqueueAsync("cat", 3, cancellationToken);<br>
  
  // adds element to the queue. Capacity = 2, available = 0, count = 2<br>
  await queue.EnqueueAsync("lion", 1);<br>
  
  // forces add element to the queue even if it's full. Capacity = 2, available = 0, count = 3<br>
  queue.ForceEnqueue("dog", 2);<br>
  
  // takes "lion" element from the queue. "lion" has highest priority. Capacity = 2, available = 0, count = 2<br>
  var element1 = await queue.DequeueAsync();<br>
  
  // takes "dog" element from the queue. "dog" has highest priority. Capacity = 2, available = 1, count = 1<br>
  var element2 = await queue.DequeueAsync();<br>
  
  // adds element to the queue. Capacity = 2, available = 0, count = 2<br>
  await queue.EnqueueAsync("bee", 3);<br>
  
  // waits 10 seconds for place in the queue.<br>
  await queue.EnqueueAsync("butterfly", 1, TimeSpan.FromSeconds(10), cancellationToken);<br>
  
  // In this place:<br>
  // If any other task dequeue an element (next is "cat") during 10 seconds then "butterfly" will be added to the queue.<br>
  // If any other task don't dequeue an element during 10 seconds TimoutException occurs.<br>
  // If any other task during 10 seconds call a cancellationTokenSource.Cancel() metod then OperationCancelledException occurs.<br>
}

Please, note that for better performance, the Enqueue and Dequeue methods are not atomic to each other but still thread safe.<br>
