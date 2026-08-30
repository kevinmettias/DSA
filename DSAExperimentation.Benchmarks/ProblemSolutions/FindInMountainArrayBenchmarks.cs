using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find in Mountain Array (LC 1095): the O(n) linear scan every one-call-at-a-time
// approach degenerates to vs. this repo's own BinarySearch.Find run twice - once per
// monotonic slope - after a hand-rolled SearchRange bisection locates the peak. The
// ascending slope holds only even values and the descending slope only odd ones, so
// _target (deep in the descending slope) can never resolve early via the ascending
// half - LinearScan is forced through nearly the whole array on every invocation,
// instead of an early exit making it look artificially competitive.
[MemoryDiagnoser]
public class FindInMountainArrayBenchmarks
{
    private static readonly IComparer<int> Descending = Comparer<int>.Create((a, b) => b.CompareTo(a));

    [Params(1_000, 100_000)]
    public int Length;

    private int[] _mountain = null!;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        var peakIndex = Length / 2;
        _mountain = new int[Length];

        for (var i = 0; i <= peakIndex; i++)
        {
            _mountain[i] = 2 * i;
        }

        for (var i = peakIndex + 1; i < Length; i++)
        {
            _mountain[i] = _mountain[i - 1] - 2;
        }

        _target = _mountain[Length - 2];
    }

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        for (var i = 0; i < _mountain.Length; i++)
        {
            if (_mountain[i] == _target)
            {
                return i;
            }
        }

        return -1;
    }

    [Benchmark]
    public int PeakBisectionThenBinarySearch() => SearchMountainArray(_mountain, _target);

    private static int SearchMountainArray(int[] mountain, int target)
    {
        var peakIndex = FindPeakIndex(mountain);

        var ascending = new OffsetSequence(mountain, offset: 0, peakIndex + 1);
        var ascendingIndex = BinarySearch.Find(ascending, target);
        if (ascendingIndex is not null)
        {
            return ascendingIndex.Value;
        }

        var descending = new OffsetSequence(mountain, peakIndex, mountain.Length - peakIndex);
        var descendingIndex = BinarySearch.Find(descending, target, Descending);

        return descendingIndex is null ? -1 : descendingIndex.Value + peakIndex;
    }

    private static int FindPeakIndex(int[] mountain)
    {
        var sequence = new OffsetSequence(mountain, offset: 0, mountain.Length);
        var range = new SearchRange(0, mountain.Length - 1);

        while (range.Low < range.High)
        {
            var mid = range.Low + ((range.High - range.Low) / AlgorithmConstants.HalvingFactor);

            range = sequence.Get(mid) < sequence.Get(mid + 1)
                ? range with { Low = mid + 1 }
                : range with { High = mid };
        }

        return range.Low;
    }

    private readonly struct OffsetSequence(int[] items, int offset, int length) : IRandomAccessSequence<int>
    {
        public int Length { get; } = length;

        public int Get(int index) => items[offset + index];
    }
}
