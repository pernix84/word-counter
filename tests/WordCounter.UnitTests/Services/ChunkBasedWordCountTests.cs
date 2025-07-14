using System.Text;
using WordCounter.Console.Abstractions;
using WordCounter.Console.Services;

namespace WordCounter.UnitTests.Services;

public class ChunkBasedWordCountTests
{
    private readonly IWordCounter _wordCounter;

    public ChunkBasedWordCountTests()
    {
        _wordCounter = new ChunkBasedWordCounter();
    }

    [Fact]
    public async Task CountWords_ThrowsArgumentNull_WhenTheStreamIsNull()
    {
        var ex = await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await _wordCounter.CountWords(null!, default);
        });

        ex.ShouldBeOfType<ArgumentNullException>();
    }

    [Theory]
    [ClassData(typeof(SimpleCorrectTestData))]
    public async Task CountWords_ReturnCorrectResults_ForSimpleCases(
        string text,
        IDictionary<string, int> expectedWordCountDictionary,
        string testCaseMessage)
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var streamReader = new StreamReader(stream);

        // Act
        var result = await _wordCounter.CountWords(streamReader, default);

        // Assert
        result.OrderBy(x => x.Key)
            .ShouldBeEquivalentTo(expectedWordCountDictionary.OrderBy(x => x.Key), testCaseMessage);
    }

    private class SimpleCorrectTestData : TheoryData<string, IDictionary<string, int>, string>
    {
        public SimpleCorrectTestData()
        {
            // Two different lines with two different casing
            var twoDifferentLinesSameWords =
                $"TEST we do have different line {Environment.NewLine}This teST is okay hopefully \t TeSt line two after tab";

            var expectedWordCountDictionary = new Dictionary<string, int>
            {
                { "test", 3 },
                { "we", 1 },
                { "do", 1 },
                { "have", 1 },
                { "different", 1 },
                { "line", 2 },
                { "this", 1 },
                { "is", 1 },
                { "okay", 1 },
                { "hopefully", 1 },
                { "two", 1 },
                { "after", 1 },
                { "tab", 1 },
            };

            Add(twoDifferentLinesSameWords, expectedWordCountDictionary, "Two words in two different lines with different capitalization should still count as the same");

            // Repeated whitespaces
            var repeatedWhitespaceText = $"Test with a lot        of whitespaces \t\t\t that we get back the test result we want ";

            var expectedWordCountDictionaryRepeatedWhitespaceText = new Dictionary<string, int>
            {
                { "test", 2 },
                { "with", 1 },
                { "a", 1 },
                { "lot", 1 },
                { "of", 1 },
                { "whitespaces", 1 },
                { "that", 1 },
                { "we", 2 },
                { "get", 1 },
                { "back", 1 },
                { "the", 1 },
                { "result", 1 },
                { "want",  1}
            };

            Add(repeatedWhitespaceText, expectedWordCountDictionaryRepeatedWhitespaceText, "Repeated whitespaces should not effect the result.");
        }
    }
}
