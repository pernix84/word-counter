using System.Collections.Concurrent;
using System.Text;
using WordCounter.Console.Abstractions;

namespace WordCounter.Console.Services;

public class ChunkBasedWordCounter : IWordCounter
{
    public async Task<IDictionary<string, int>> CountWords(StreamReader streamReader, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(streamReader);

        ConcurrentDictionary<string, int> wordCountBag = new();

        var wordBuffer = new StringBuilder(100);
        var memoryBuffer = (new char[4096]).AsMemory();
        int charsRead;

        while ((charsRead = await streamReader.ReadAsync(memoryBuffer, cancellationToken)) > 0)
        {
            var span = memoryBuffer.Span[..charsRead];
            
            foreach (char c in span)
            {
                if (char.IsWhiteSpace(c)) // May add additional delimiters later
                {
                    if (wordBuffer.Length > 0)
                    {
                        var word = wordBuffer.ToString().ToLower();
                        wordCountBag.AddOrUpdate(word, 1, (_, count) => count + 1);
                        wordBuffer.Clear();
                    }
                }
                else
                {
                    wordBuffer.Append(c);
                }
            }
        }

        // The last word of the file
        if (wordBuffer.Length > 0)
        {
            var word = wordBuffer.ToString();
            wordCountBag.AddOrUpdate(word, 1, (_, count) => count + 1);
        }

        return wordCountBag.ToDictionary();
    }
}
