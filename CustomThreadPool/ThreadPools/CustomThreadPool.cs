namespace CustomThreadPool.ThreadPools;

public class CustomThreadPool : IThreadPool
{
    private readonly Queue<Action> _queue = new Queue<Action>();
    private long _processedTaskCount = 0;

    public CustomThreadPool()
    {
        StartBckThreads(Worker, 16);
    }

    public void EnqueueAction(Action action)
    {
        lock (_queue)
        {
            _queue.Enqueue(action);
            Monitor.Pulse(_queue);
        }
    }

    public long GetTasksProcessedCount()
    {
        return _processedTaskCount;
    }

    private void Worker()
    {
        while (true)
        {
            lock (_queue)
            {
                if (_queue.TryDequeue(out var action))
                {
                    action.Invoke();
                    Interlocked.Increment(ref _processedTaskCount);
                }
                else
                {
                    Monitor.Wait(_queue);
                }
            }
        }
    }

    private static Thread[] StartBckThreads(Action action, int count)
    {
        return Enumerable.Range(0, count).Select(_ => StartBckThread(action)).ToArray();
    }

    private static Thread StartBckThread(Action action)
    {
        var thread = new Thread(() => action())
        {
            IsBackground = true
        };

        thread.Start();
        return thread;
    }
}