using System.Collections.Concurrent;
using LogParsing.LogParsers;

namespace DataParallelism.LogParsers;

public class ParallelLogParser : ILogParser
{
    private readonly FileInfo file;
    private readonly Func<string, string?> tryGetIdFromLine;

    public ParallelLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
    {
        this.file = file;
        this.tryGetIdFromLine = tryGetIdFromLine;
    }

    public string?[] GetRequestedIdsFromLogFile()
    {
        var lines = File.ReadLines(file.FullName);
        var concurrentBag = new ConcurrentBag<string?>();
        Parallel.ForEach(lines, line =>
        {
            var id = tryGetIdFromLine(line);
            if (id != null) concurrentBag.Add(tryGetIdFromLine(line));
        });
        return concurrentBag.ToArray();
    }
}