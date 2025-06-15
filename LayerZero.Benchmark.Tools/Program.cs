// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LayerZero.Benchmark.Tools.Collections;
using LayerZero.Benchmark.Tools.CoreClasses;
using LayerZero.Benchmark.Tools.Guards;
using LayerZero.Benchmark.Tools.IO;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());

BenchmarkRunner.Run<SafeLinqBenchmark>();
BenchmarkRunner.Run<SygilParserBenchmark>();
BenchmarkRunner.Run<SpindleTreeGuardBenchmark>();
BenchmarkRunner.Run<SpindleTreeBenchmark>();



Console.ReadKey();
