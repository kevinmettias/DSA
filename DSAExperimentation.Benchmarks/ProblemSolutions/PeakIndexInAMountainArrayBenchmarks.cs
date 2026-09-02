using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Peak Index in a Mountain Array (LC 852): LinearScan walks the array until it
// finds the downhill step, O(n). BinarySearchLowerBound instead encodes "is this
// step downhill" as a 0/1 predicate sequence (monotonic because the array is
// unimodal) and reuses this repo's own BinarySearch.LowerBound, O(log n) - the
// same proxy-sequence idiom FindMinimumInRotatedSortedArrayTests uses for its
// own pivot search.
[MemoryDiagnoser]
public class PeakIndexInAMountainArrayBenchmarks
{
    private const int MidpointDivisor = 2;

    [Params(200, 100_000)]
    public int Length;

    private int[] _mountain = null!;

    [GlobalSetup]
    public void Setup()
    {
        _mountain = new int[Length];
        var peak = Length / MidpointDivisor;

        for (var i = 0; i < Length; i++)
        {
            _mountain[i] = i <= peak ? i : Length - i;
        }
    }

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        for (var i = 0; i < _mountain.Length - 1; i++)
        {
            if (_mountain[i] > _mountain[i + 1])
            {
                return i;
            }
        }

        return _mountain.Length - 1;
    }

    [Benchmark]
    public int BinarySearchLowerBound()
        => BinarySearch.LowerBound<int, DescendingStepSequence>(new DescendingStepSequence(_mountain), 1);

    private readonly struct DescendingStepSequence(int[] mountain) : IRandomAccessSequence<int>
    {
        public int Length => mountain.Length - 1;

        public int Get(int index) => mountain[index] > mountain[index + 1] ? 1 : 0;
    }
}
