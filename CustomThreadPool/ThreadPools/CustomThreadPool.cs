using System.Collections.ObjectModel;

namespace CustomThreadPool.ThreadPools;

public class CustomThreadPool : IThreadPool
{
    private readonly Queue<Action> _queue = new Queue<Action>();

    private readonly IReadOnlyDictionary<int, WorkStealingQueue<Action>> LocalQueues;

    private readonly Dictionary<int, WorkStealingQueue<Action>> _tempDict =
        new Dictionary<int, WorkStealingQueue<Action>>();

    private long _processedTaskCount = 0;

    public CustomThreadPool()
    {
        StartBckThreads(Worker, 16);
        LocalQueues = new ReadOnlyDictionary<int, WorkStealingQueue<Action>>(_tempDict);
    }


    public void EnqueueAction(Action action)
    {
        lock (_queue)
        {
            _queue.Enqueue(action);
            // Monitor.Pulse(_queue);
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
            var action = () => { };
            if (LocalQueues[Environment.CurrentManagedThreadId].LocalPop(ref action))
            {
                action.Invoke();
                Interlocked.Increment(ref _processedTaskCount);
            }

            if (!TryDequeueFromGeneralQueue())
            {
                TrySteel();
            }
        }
    }

    private bool TrySteel()
    {
        // foreach (var threadId in LocalQueues.Keys)
        // {
        //     if (threadId == Environment.CurrentManagedThreadId) continue;
        //     var action = () => { };
        //     if (!LocalQueues[threadId].TrySteal(ref action)) continue;
        //     LocalQueues[Environment.CurrentManagedThreadId].LocalPush(action);
        //     return true;
        // }

        foreach (var workStealingQueue in LocalQueues)
        {
            var action = () => { };
            if (!workStealingQueue.Value.TrySteal(ref action)) continue;
            LocalQueues[Environment.CurrentManagedThreadId].LocalPush(action);
        }

        return false;
    }

    private bool TryDequeueFromGeneralQueue()
    {
        lock (_queue)
        {
            if (_queue.TryDequeue(out var action))
            {
                LocalQueues[Environment.CurrentManagedThreadId].LocalPush(action);
                return true;
            }

            return false;
        }
    }

    private Thread[] StartBckThreads(Action action, int count)
    {
        return Enumerable.Range(0, count).Select(_ => StartBckThread(action)).ToArray();
    }

    private Thread StartBckThread(Action action)
    {
        var thread = new Thread(() => action())
        {
            IsBackground = true
        };
        _tempDict.Add(thread.ManagedThreadId, new WorkStealingQueue<Action>());
        thread.Start();
        // Console.WriteLine(thread.ManagedThreadId);
        return thread;
    }
}