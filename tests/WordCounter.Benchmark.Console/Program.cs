// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using WordCounter.Benchmark.Console;

Console.WriteLine("Hello, World!");

var summary = BenchmarkRunner.Run<WordCounterBenchmark>();
