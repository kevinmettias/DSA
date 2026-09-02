using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// First Bad Version (LC 278): an O(n) linear IsBadVersion scan vs. this repo's own
// BinarySearch.LowerBound over a monotone virtual sequence (SqrtXBenchmarks' shape) -
// O(log n) probes instead of walking the whole version history. FirstBad sits at 70%
// of VersionCount so the linear scan pays close to its full O(n) worst case every call.
[MemoryDiagnoser]
public class FirstBadVersionBenchmarks
{
    private const double FirstBadFraction = 0.7;

    [Params(1_000, 1_000_000)]
    public int VersionCount;

    private int _firstBad;

    [GlobalSetup]
    public void Setup() => _firstBad = (int)(VersionCount * FirstBadFraction);

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        for (var version = 1; version <= VersionCount; version++)
        {
            if (IsBadVersion(version))
            {
                return version;
            }
        }

        return -1;
    }

    [Benchmark]
    public int BinarySearchLowerBound()
    {
        var sequence = new IsBadVersionSequence(_firstBad, VersionCount);
        return BinarySearch.LowerBound<int, IsBadVersionSequence>(sequence, 1) + 1;
    }

    private bool IsBadVersion(int version) => version >= _firstBad;

    private readonly struct IsBadVersionSequence(int firstBad, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => index + 1 >= firstBad ? 1 : 0;
    }
}
