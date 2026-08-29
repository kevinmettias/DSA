using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SearchInRotatedSortedArrayBenchmarks
{
    private int[] _values = null!;
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() { var sorted = Enumerable.Range(0, Length).ToArray(); var pivot = Length / 3; _values = sorted[pivot..].Concat(sorted[..pivot]).ToArray(); }
    [Benchmark(Baseline = true)] public int LinearScan() => Array.IndexOf(_values, Length - 2);
    [Benchmark] public int BinarySearchPivotAndSlice() { var target = Length - 2; var pivot = BinarySearch.LowerBound<int, PivotSequence>(new PivotSequence(_values), 1); var searchRight = target <= _values[^1]; var start = searchRight ? pivot : 0; var length = searchRight ? _values.Length - pivot : pivot; var found = BinarySearch.Find<int, OffsetSequence>(new OffsetSequence(_values, start, length), target); return found is null ? -1 : start + found.Value; }
    private readonly struct PivotSequence(int[] nums) : IRandomAccessSequence<int> { public int Length => nums.Length; public int Get(int index) => nums[index] <= nums[^1] ? 1 : 0; }
    private readonly struct OffsetSequence(int[] nums, int start, int length) : IRandomAccessSequence<int> { public int Length => length; public int Get(int index) => nums[start + index]; }
}
