using BenchmarkDotNet.Attributes;
using LayerZero.Tools.Guard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Benchmark.Tools.Guards
{
    [MemoryDiagnoser]
    public class SpindleTreeGuardBenchmark
    {
        private string _emptyDir;
        private string _nonEmptyDir;
        private string _txtOnlyDir;

        [GlobalSetup]
        public void Setup()
        {
            _emptyDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_emptyDir);

            _nonEmptyDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_nonEmptyDir);
            File.WriteAllText(Path.Combine(_nonEmptyDir, "file.txt"), "content");

            _txtOnlyDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_txtOnlyDir);
            File.WriteAllText(Path.Combine(_txtOnlyDir, "note.txt"), "just text");
        }

        [Benchmark]
        public bool EmptyDirectory() => SpindleTreeGuard.IsDirectoryEmpty(_emptyDir);

        [Benchmark]
        public bool NonEmptyDirectory() => SpindleTreeGuard.IsDirectoryEmpty(_nonEmptyDir);

        [Benchmark]
        public bool TxtFilter_Match() => SpindleTreeGuard.IsDirectoryEmpty(_txtOnlyDir, SearchOption.AllDirectories, new[] { ".txt" });

        [Benchmark]
        public bool LogFilter_NoMatch() => SpindleTreeGuard.IsDirectoryEmpty(_txtOnlyDir, SearchOption.AllDirectories, new[] { ".log" });
    }
}
