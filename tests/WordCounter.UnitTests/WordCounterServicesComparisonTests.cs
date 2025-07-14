using System.IO.Abstractions;
using WordCounter.Console.Services;

namespace WordCounter.UnitTests;

public class WordCounterServiceComparisonIntegrationTests
{
    [Theory]
    [ClassData(typeof(WordCounterServicesComparisonTestTheoryData))]
    public async Task Test1(IEnumerable<string> filePaths) // Integration
    {
        // Arrange
        var simpleWordCounterService = new SimpleWordCounterService(
            new LineBasedWordCounter(),
            new FileSystem()
        );

        var advancedWordCounter = new ParallelWordCounterService(
            new ChunkBasedWordCounter(),
            new FileSystem()
        );

        // Act
        var resultFromSimple = await simpleWordCounterService.CountWords(filePaths, default);
        var resultFromAdvanced = await advancedWordCounter.CountWords(filePaths, default);

        // Assert
        resultFromSimple
            .OrderBy(x => x.Key)
            .ShouldBeEquivalentTo(resultFromAdvanced.OrderBy(x => x.Key));
    }

    private class WordCounterServicesComparisonTestTheoryData : TheoryData<IEnumerable<string>>
    {
        public WordCounterServicesComparisonTestTheoryData()
        {
            List<string> simpleFilePaths =
            [
                "example_file1.txt",
                "example_file2.txt",
            ];

            Add(simpleFilePaths);

            List<string> complexFilePaths =
            [
                "generated_1.txt",
                "generated_2.txt",
                "generated_3.txt",
            ];

            Add(complexFilePaths);
        }
    }
}
