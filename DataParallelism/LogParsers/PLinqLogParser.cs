using LogParsing.LogParsers;

namespace DataParallelism.LogParsers;

public class PLinqLogParser : ILogParser
{
    private readonly FileInfo file;
    private readonly Func<string, string?> tryGetIdFromLine;
    public PLinqLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
    {
        this.file = file;
        this.tryGetIdFromLine = tryGetIdFromLine;
    }

    public string?[] GetRequestedIdsFromLogFile()
    {
        var lines = File.ReadLines(file.FullName);
        return lines
            .AsParallel()
            .Select(tryGetIdFromLine)
            .Where(id => id != null)
            .ToArray();
    }
}