using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using WordCounter.Console.Abstractions;
using WordCounter.Console.Services;

namespace WordCounter.UnitTests.Services;

public class WordCounterServiceTests
{
    private readonly IFileWordCounterService _wordCounterService;
    private readonly Mock<IWordCounter> _wordCounterMock = new();
    private readonly Mock<IFileSystem> _fileSystemMock = new();
    private readonly MockFileSystem _mockFileSystem;
    private readonly Mock<IFile> _fileMock = new();

    private readonly Fixture _fixture = new();

    public WordCounterServiceTests()
    {
        _mockFileSystem = new MockFileSystem();

        _fileMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileMock.Setup(x => x.OpenRead(It.IsAny<string>()))
            .Returns<string>(path => _mockFileSystem.File.OpenRead(path));
        _fileSystemMock.Setup(x => x.File)
            .Returns(_fileMock.Object);

        _wordCounterService = new ParallelWordCounterService
        (
            _wordCounterMock.Object,
            _fileSystemMock.Object
        );
    }

    [Fact]
    public async Task CountWords_WithInvalidFiles()
    {
        // Arrange
        var validCount = 2;
        var invalidCount = 3;
        var fileNames = _fixture.CreateMany<string>(validCount + invalidCount);

        var sequence = _fileMock.SetupSequence(x => x.Exists(It.IsAny<string>()));

        for (int i = 0; i < validCount; i++)
        {
            sequence = sequence.Returns(true);
        }
        for (int i = 0; i < invalidCount; i++)
        {
            sequence = sequence.Returns(false);
        }

        SetupFileSystemMock(fileNames);

        _wordCounterMock.Setup(x => x.CountWords(It.IsAny<StreamReader>(), default))
            .ReturnsAsync(new Dictionary<string, int>());

        // Act
        var result = await _wordCounterService.CountWords(fileNames, default);

        // Assert
        _wordCounterMock.Verify(x => x.CountWords(It.IsAny<StreamReader>(), default), Times.Exactly(validCount));
            
    }

    [Fact]
    public async Task CountWords_WithComplexScenario_ReturnsGaussianSum()
    {
        // Arrange
        var fileCount = _fixture.Create<int>() % 20 + 1; // 1-20 numbers
        var fileNames = _fixture.CreateMany<string>(fileCount); // It doesn't matter. We simulate the file system.
        var threeRandomWords = _fixture.CreateMany<string>(3);

        SetupFileSystemMock(fileNames);

        var sequence = _wordCounterMock.SetupSequence(x => x.CountWords(It.IsAny<StreamReader>(), default));
        for (int i = 0; i < fileCount; i++)
        {
            sequence = sequence.ReturnsAsync(threeRandomWords.ToDictionary(x => x, x => i + 1));
        }

        // Act
        var result = await _wordCounterService.CountWords(fileNames, default);

        // Assert
        var expectedCountOfAllWords = fileCount * (fileCount + 1) / 2;
        result.ShouldAllBe(x => x.Value == expectedCountOfAllWords);
        result.Keys.Count.ShouldBe(3);
    }

    private void SetupFileSystemMock(IEnumerable<string> fileNames)
    {
        var fileSystem = new MockFileSystem(
            fileNames.ToDictionary(fn => fn,
                _ => new MockFileData(_fixture.Create<string>())));

        _fileMock.Setup(x => x.OpenRead(It.IsAny<string>()))
            .Returns<string>(fileSystem.File.OpenRead);
    }
}
