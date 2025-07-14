// See https://aka.ms/new-console-template for more information
using System.IO.Abstractions;
using System.Text.Json;
using WordCounter.Console.Services;

var filePaths = args.Length > 1 ? args : GetExampleFilePaths();
var existingFilePaths = filePaths.Where(File.Exists);

var parallelWordCounterService = new ParallelWordCounterService(
    new ChunkBasedWordCounter(),
    new FileSystem()
);

var wordCountDictionary = await parallelWordCounterService.CountWords(existingFilePaths, default);
var indentedJsonText = JsonSerializer.Serialize(wordCountDictionary, new JsonSerializerOptions
{
    WriteIndented = true
});
File.WriteAllText("word_count_result.json", indentedJsonText);

// This is purely for the sake of example
static IEnumerable<string> GetExampleFilePaths()
{
    List<string> exampleFiles =
    [
        "example_file1.txt",
        "example_file2.txt",
        "generated_1.txt",
        "generated_2.txt",
        "generated_3.txt",
    ];

    if (exampleFiles.All(path => !File.Exists(path)))
    {
        var files = Directory.GetFiles("./files/", "*.txt");
        return files;
    }

    return exampleFiles;
}
