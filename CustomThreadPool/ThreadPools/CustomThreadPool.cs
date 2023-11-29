using System.Collections.ObjectModel;

namespace CustomThreadPool.ThreadPools;

public class CustomThreadPool : IThreadPool
{
    private readonly Queue<Action> queue = new();

    private readonly IReadOnlyDictionary<int, WorkStealingQueue<Action>> localQueues;

    private readonly Dictionary<int, WorkStealingQueue<Action>> tempDict = new();

    private long processedTaskCount;

    public CustomThreadPool(int concurrencyLevel)
    {
        var threads = CreateBckThreads(Worker, concurrencyLevel);
        localQueues = new ReadOnlyDictionary<int, WorkStealingQueue<Action>>(tempDict);
        StartAllThreads(threads);
    }

    public CustomThreadPool() : this(Environment.ProcessorCount)
    {
    }

    public void EnqueueAction(Action action)
    {
        lock (queue)
        {
            queue.Enqueue(action);
        }
    }

    public long GetTasksProcessedCount() => processedTaskCount;

    private void Worker()
    {
        while (true)
        {
            var action = () => { };
            if (localQueues[Environment.CurrentManagedThreadId].LocalPop(ref action))
            {
                action.Invoke();
                Interlocked.Increment(ref processedTaskCount);
            }

            if (!TryDequeueFromGeneralQueue())
            {
                TrySteel();
            }
        }
    }

    private bool TrySteel()
    {
        foreach (var threadId in localQueues.Keys)
        {
            if (threadId == Environment.CurrentManagedThreadId) continue;
            var action = () => { };
            if (!localQueues[threadId].TrySteal(ref action)) continue;
            localQueues[Environment.CurrentManagedThreadId].LocalPush(action);
            return true;
        }

        return false;
    }

    private bool TryDequeueFromGeneralQueue()
    {
        lock (queue)
        {
            if (queue.TryDequeue(out var action))
            {
                localQueues[Environment.CurrentManagedThreadId].LocalPush(action);
                return true;
            }

            return false;
        }
    }

    private IEnumerable<Thread> CreateBckThreads(Action action, int count)
    {
        return Enumerable.Range(0, count).Select(_ => StartBckThread(action)).ToArray();
    }

    private Thread StartBckThread(Action action)
    {
        var thread = new Thread(() => action())
        {
            IsBackground = true
        };
        tempDict.Add(thread.ManagedThreadId, new WorkStealingQueue<Action>());
        // thread.Start();
        // Console.WriteLine(thread.ManagedThreadId);
        return thread;
    }

    private static void StartAllThreads(IEnumerable<Thread> threads)
    {
        foreach (var thread in threads)
            thread.Start();
    }
}