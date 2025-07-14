using System.Collections.Concurrent;
using System.IO.Abstractions;
using System.Text;
using WordCounter.Console.Abstractions;

namespace WordCounter.Console.Services;

public class ParallelWordCounterService(
    IWordCounter wordCounter,
    IFileSystem fileSystem
)
: IFileWordCounterService
{
    public async Task<IDictionary<string, int>> CountWords(IEnumerable<string> filePaths, CancellationToken cancellationToken)
    {
        ConcurrentDictionary<string, int> result = new();

        // Limit the maximum number of files handled at a moment
        var parallelizationOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 5
        };

        await Parallel.ForEachAsync(filePaths, parallelizationOptions, async (path, ct) =>
        {
            if (!fileSystem.File.Exists(path))
            {
                System.Console.WriteLine($"File doesn't exist at '{path}'");
                return;
            }

            using var fileStream = fileSystem.File.OpenRead(path);
            using var reader = new StreamReader(fileStream, Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 65536); // 1KB is default

            var wordDictionary = await wordCounter.CountWords(reader, cancellationToken);

            foreach (var kvp in wordDictionary)
            {
                result.AddOrUpdate(kvp.Key, kvp.Value, (_, count) => count + kvp.Value);
            }
        });

        return result;
    }
}
