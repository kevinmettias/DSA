using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Search in Rotated Sorted Array II (LC 81): a plain O(n) linear scan vs. trimming
// duplicate boundary values off the left edge and handing the remaining slice to
// this repo's own BinarySearch.LowerBound/Find, the same pivot-then-bisect strategy
// SearchInRotatedSortedArrayBenchmarks (LC 33) already uses. A bounded band of
// duplicate values is stamped across both ends so the trim loop does real, but
// small, work relative to Length - large enough to exercise duplicate handling,
// small enough that the O(log n) win over LinearScan still shows.
[MemoryDiagnoser]
public class SearchInRotatedSortedArrayIIBenchmarks
{
    private int[] _values = null!;
    private int _target;

    [Params(200, 5_000)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        var sorted = Enumerable.Range(0, Length).ToArray();
        var pivot = Length / 3;
        var rotated = sorted[pivot..].Concat(sorted[..pivot]).ToArray();

        var duplicateSpan = Math.Min(Length / 8, 40);
        for (var i = 0; i < duplicateSpan; i++)
        {
            rotated[i] = rotated[0];
            rotated[^(i + 1)] = rotated[0];
        }

        _values = rotated;
        _target = pivot / 2;
    }

    [Benchmark(Baseline = true)]
    public bool LinearScan() => Array.IndexOf(_values, _target) >= 0;

    [Benchmark]
    public bool TrimDuplicatesThenBinarySearch()
    {
        var left = 0;
        var right = _values.Length - 1;

        while (left < right && _values[left] == _values[right])
        {
            left++;
        }

        var trimmedLength = right - left + 1;
        var trimmedLast = _values[right];

        var pivot = BinarySearch.LowerBound<int, PivotSequence>(new PivotSequence(_values, left, trimmedLength, trimmedLast), 1);
        var searchRight = _target <= trimmedLast;
        var start = searchRight ? pivot : 0;
        var length = searchRight ? trimmedLength - pivot : pivot;
        var found = BinarySearch.Find<int, OffsetSequence>(new OffsetSequence(_values, left + start, length), _target);

        return found is not null;
    }

    private readonly struct PivotSequence(int[] nums, int start, int length, int lastValue) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int index) => nums[start + index] <= lastValue ? 1 : 0;
    }

    private readonly struct OffsetSequence(int[] nums, int start, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int index) => nums[start + index];
    }
}
