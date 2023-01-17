using System.Runtime.CompilerServices;

namespace Kulaszka.Collections
{
    public static class WaitHandleExtension
    {
        public static TaskAwaiter GetAwaiter(this WaitHandle handle)
        {
            return handle.ToTask().GetAwaiter();
        }

        public static Task ToTask(this WaitHandle handle, int millisecondsTimeout = Timeout.Infinite)
        {
            var taskCompletionSource = new TaskCompletionSource<object?>();
            var localVariableInitLock = new object();
            
            lock (localVariableInitLock)
            {
                RegisteredWaitHandle? callbackHandle = null;
                callbackHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) =>
                    {
                        ((TaskCompletionSource<object?>)state).SetResult(null);

                        lock (localVariableInitLock)
                        {
                            callbackHandle.Unregister(null);
                        }
                    },
                    taskCompletionSource,
                    millisecondsTimeout,
                    true);
            }

            return taskCompletionSource.Task;
        }

        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            RegisteredWaitHandle? registeredHandle = null;
            CancellationTokenRegistration tokenRegistration = default;
            
            try
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    taskCompletionSource,
                    millisecondsTimeout,
                    true);
                
                tokenRegistration = cancellationToken.Register(
                    state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
                    taskCompletionSource);
                
                return await taskCompletionSource.Task;
            }
            finally
            {
                if (registeredHandle != null)
                {
                    registeredHandle.Unregister(null);
                }
                
                tokenRegistration.Dispose();
            }
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken)
        {
            return handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
        }
    }
}
