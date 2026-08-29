using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SearchInsertPositionBenchmarks
{
    private int[] _values = null!;
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i * 2).ToArray();
    [Benchmark(Baseline = true)] public int LinearScan() { var target = (Length * 2) - 1; for (var i = 0; i < _values.Length; i++) if (_values[i] >= target) return i; return _values.Length; }
    [Benchmark] public int BinarySearchLowerBound() => BinarySearch.LowerBound(new ArraySequence<int>(_values), (Length * 2) - 1);
}
