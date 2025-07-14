using System.IO.Abstractions;
using BenchmarkDotNet.Attributes;
using WordCounter.Console.Abstractions;
using WordCounter.Console.Services;

namespace WordCounter.Benchmark.Console;

[MemoryDiagnoser]
[SimpleJob]
public class WordCounterBenchmark
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private IFileWordCounterService _simpleSimpleService;
    private IFileWordCounterService _simpleChunkService;
    private IFileWordCounterService _advancedSimpleService;
    private IFileWordCounterService _advancedChunkService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [GlobalSetup]
    public void Setup()
    {
        _simpleSimpleService = new SimpleWordCounterService(
            new LineBasedWordCounter(),
            new FileSystem());
        _simpleChunkService = new SimpleWordCounterService(
            new ChunkBasedWordCounter(),
            new FileSystem());
        _advancedSimpleService = new ParallelWordCounterService(
            new LineBasedWordCounter(),
            new FileSystem());
        _advancedChunkService = new ParallelWordCounterService(
            new ChunkBasedWordCounter(),
            new FileSystem());
    }

    [Benchmark]
    public async Task SimpleWithSimple()
    {
        var _ = await _simpleSimpleService.CountWords(FilePaths, default);
    }

    [Benchmark]
    public async Task SimpleWithChunk()
    {
        var _ = await _simpleChunkService.CountWords(FilePaths, default);
    }

    [Benchmark]
    public async Task AdvancedWithSimple()
    {
        var _ = await _advancedSimpleService.CountWords(FilePaths, default);
    }

    [Benchmark]
    public async Task AdvancedWithChunk()
    {
        var _ = await _advancedChunkService.CountWords(FilePaths, default);
    }

    private static IEnumerable<string> FilePaths =>
    [
        "pg23.txt",
        "pg147.txt",
        "pg851.txt",
        "pg14977.txt",
        "pg20203.txt",
        "pg29870.txt",
        "pg29870_gigantic.txt",
        "pg29870_gigantic_2.txt",
        "pg29870_gigantic_3.txt",
        "pg29870_gigantic_4.txt",
        "pg29870_gigantic_5.txt",
        "pg29870_gigantic_6.txt",
        "pg29870_gigantic_7.txt",
        "pg29870_gigantic_8.txt",
        "pg34049.txt",
        "pg41267.txt",
        "pg45502.txt",
        "pg45631.txt",
    ];
}
