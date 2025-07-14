using System.Text.RegularExpressions;
using WordCounter.Console.Abstractions;

namespace WordCounter.Console.Services;

public class LineBasedWordCounter : IWordCounter
{
    public async Task<IDictionary<string, int>> CountWords(StreamReader streamReader, CancellationToken cancellationToken)
    {
        Dictionary<string, int> wordCountsBag = new();
        string line;

        while ((line = (await streamReader.ReadLineAsync(cancellationToken))!) is not null)
        {
            var split = Regex.Replace(line, @"\s+", " ")
                .ToLower()  
                .Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in split)
            {
                if (!wordCountsBag.ContainsKey(word))
                    wordCountsBag.Add(word, 1);
                else
                    wordCountsBag[word]++;
            }
        }

        return wordCountsBag;
    }
}
