// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LayerZero.Benchmark.Tools.Collections;
using LayerZero.Benchmark.Tools.CoreClasses;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());

BenchmarkRunner.Run<SafeLinqBenchmark>();
BenchmarkRunner.Run<SygilParserBenchmark>();



Console.ReadKey();
