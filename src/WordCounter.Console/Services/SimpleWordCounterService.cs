using System.Collections.Concurrent;
using System.IO.Abstractions;
using System.Text;
using WordCounter.Console.Abstractions;

namespace WordCounter.Console.Services;

public class SimpleWordCounterService(
    IWordCounter wordCounter,
    IFileSystem fileSystem
)
: IFileWordCounterService
{
    public async Task<IDictionary<string, int>> CountWords(IEnumerable<string> filePaths, CancellationToken cancellationToken)
    {
        ConcurrentDictionary<string, int> results = new();

        var tasks = filePaths
            .Where(fileSystem.File.Exists)
            .Select(p =>
                Task.Run(async () =>
                {
                    using var reader = new StreamReader(p, Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: true,
                        bufferSize: 65536); // Use larger buffer for speed

                    var res = await wordCounter.CountWords(reader, cancellationToken);

                    return res;
                }, cancellationToken));

        var result = await Task.WhenAll(tasks);

        var mergedWordCounts = result.SelectMany(dict => dict)
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));

        return mergedWordCounts;
    }
}
