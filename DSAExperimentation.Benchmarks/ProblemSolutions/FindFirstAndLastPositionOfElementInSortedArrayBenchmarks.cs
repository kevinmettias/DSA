using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class FindFirstAndLastPositionOfElementInSortedArrayBenchmarks
{
    private int[] _values = null!;
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / 4).ToArray();
    [Benchmark(Baseline = true)] public int LinearRange() { var first = Array.IndexOf(_values, Length / 8); var last = Array.LastIndexOf(_values, Length / 8); return first + last; }
    [Benchmark] public int BinarySearchBounds() { var sequence = new ArraySequence<int>(_values); var target = Length / 8; return BinarySearch.LowerBound(sequence, target) + BinarySearch.UpperBound(sequence, target) - 1; }
}
