using BenchmarkDotNet.Attributes;
using LayerZero.Tools.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Benchmark.Tools.IO
{
    [MemoryDiagnoser]
    public class SpindleTreeBenchmark
    {
        private string _benchmarkDir;

        [GlobalSetup]
        public void Setup()
        {
            _benchmarkDir = Path.Combine(Path.GetTempPath(), "SpindleTreeBench");
            Directory.CreateDirectory(_benchmarkDir);

            for (int i = 0; i < 100; i++)
            {
                File.WriteAllText(Path.Combine(_benchmarkDir, $"file_{i}.js"), "// js content");
            }
        }

        [Benchmark]
        public List<string>? GetAllJsFiles()
        {
            return SpindleTree.GetAllFilesPath(_benchmarkDir, FileExtensions: new[] { ".js" });
        }

        [Benchmark]
        public List<string>? GetAllFiles_NoFilter()
        {
            return SpindleTree.GetAllFilesPath(_benchmarkDir);
        }
    }
}
