using BenchmarkDotNet.Attributes;
using LayerZero.Tools.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Benchmark.Tools.Collections
{
    [MemoryDiagnoser]
    public class SafeLinqBenchmark
    {
        private List<int> source;
        private List<int> emptySource;
        private Dictionary<int, int> prebuiltDict;

        [GlobalSetup]
        public void Setup()
        {
            source = Enumerable.Range(0, 10000).ToList();
            emptySource = new List<int>();
            prebuiltDict = source.ToDictionary(i => i);
        }

        // === ElementAtSafe ===
        [Benchmark]
        public int? SafeElementAt_Middle()
            => SafeLinQ<int>.ElementAtSafe(source, 5000);

        // === ToDictionarySafe ===
        [Benchmark]
        public Dictionary<int, int> ToDictionarySafe_NoOverwrite()
            => SafeLinQ<int>.ToDictionarySafe(source, x => x, false);

        [Benchmark]
        public Dictionary<int, int> ToDictionarySafe_WithOverwrite()
            => SafeLinQ<int>.ToDictionarySafe(source.Concat(source), x => x, true);

        // === DistinctSafe ===
        [Benchmark]
        public IEnumerable<int> DistinctSafe_WithDuplicates()
            => SafeLinQ<int>.DistinctSafe(source.Concat(source)).ToList();

        // === FallbackIfEmpty ===
        [Benchmark]
        public IEnumerable<int> FallbackIfEmpty_EmptySource()
            => SafeLinQ<int>.FallbackIfEmpty(emptySource, () => -1).ToList();

        [Benchmark]
        public IEnumerable<int> FallbackIfEmpty_NonEmptySource()
            => SafeLinQ<int>.FallbackIfEmpty(source, () => -1).ToList();
    }
}
