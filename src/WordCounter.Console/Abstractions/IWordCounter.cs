namespace WordCounter.Console.Abstractions;

public interface IWordCounter
{
    /// <summary>
    /// Takes a <see cref="StreamReader"/> object and counts the words in it.
    /// </summary>
    /// <param name="streamReader"></param>
    /// <returns>A <see cref="IDictionary{string, int}"/> of the words and their counts</returns>
    Task<IDictionary<string, int>> CountWords(StreamReader streamReader, CancellationToken cancellationToken);
}
