namespace WordCounter.Console.Abstractions;

/// <summary>
/// Counts words from a list of files.
/// </summary>
public interface IFileWordCounterService
{
    /// <summary>
    /// Counts the words in the given filepaths.
    /// </summary>
    /// <param name="filePaths"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="IDictionary{string, int}"/> of the words and their counts aggregated by all files.</returns>
    Task<IDictionary<string, int>> CountWords(IEnumerable<string> filePaths, CancellationToken cancellationToken);
}
