using System.Collections.Concurrent;
using LogParsing.LogParsers;

namespace DataParallelism.LogParsers;

public class ThreadLogParser : ILogParser
{
    private readonly FileInfo file;
    private readonly Func<string, string?> tryGetIdFromLine;
    private ConcurrentQueue<string> queue;
    private ConcurrentBag<string?> result;
    private List<Thread> threadList = new();

    public ThreadLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
    {
        this.file = file;
        this.tryGetIdFromLine = tryGetIdFromLine;
    }

    private void Executable()
    {
        while (queue.TryDequeue(out var line))
        {
            var id = tryGetIdFromLine(line);
            if (id != null) result.Add(id);
        }
    }

    public string?[] GetRequestedIdsFromLogFile()
    {
        var lines = File.ReadLines(file.FullName);
        queue = new ConcurrentQueue<string>(lines);
        result = new ConcurrentBag<string?>();
        var processCount = Environment.ProcessorCount;
        
        for (int i = 0; i < processCount; i++)
        {
            var thread = new Thread(Executable);
            threadList.Add(thread);
            thread.Start();
        }

        foreach (var thread in threadList)
            thread.Join();

        return result.ToArray();
    }
}