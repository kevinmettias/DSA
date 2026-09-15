using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.OnlineMajorityElementInSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OnlineMajorityElementInSubarraySolution's, the same
// classes OnlineMajorityElementInSubarrayTests proves correct - a naive per-query
// O(range) tally against this repo's own HashMap<int, DynamicArray<int>> position
// index, which counts a candidate's occurrences in O(log n) via
// BinarySearch.LowerBound/UpperBound over a DynamicArraySequence<int> view. Both
// checkers are constructed in [GlobalSetup], so building the position index is
// charged to setup rather than to the query replay each arm measures.
//
// Every generated query's range sits entirely inside one same-valued run, so its
// answer is that run's value with an occurrence count equal to the whole range - a
// majority satisfying LeetCode's own 2*threshold > range guarantee for every query,
// which also means the indexed arm's first sample always lands on the answer.
[MemoryDiagnoser]
public class OnlineMajorityElementInSubarrayBenchmarks
{
    private const int RunLength = 25;
    private const int QueryCount = 200;
    private const int RandomSeed = 1; private (int Left, int Right, int Threshold)[] _queries = [];

    private OnlineMajorityElementInSubarraySolution.IMajorityChecker _rangeTally = null!;
    private OnlineMajorityElementInSubarraySolution.IMajorityChecker _positionIndex = null!;
    // unchanged from the pre-migration workload

    [Params(1_000, 8_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = BuildRunLengthEncodedArray(Length);
        _queries = BuildQueriesWithinRuns(Length);
        _rangeTally = new OnlineMajorityElementInSubarraySolution.MajorityCheckerByRangeTally(values);
        _positionIndex = new OnlineMajorityElementInSubarraySolution.MajorityCheckerByPositionIndex(values);
    }

    private static int[] BuildRunLengthEncodedArray(int length)
    {
        var values = new int[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = i / RunLength;
        }

        return values;
    }

    private static (int Left, int Right, int Threshold)[] BuildQueriesWithinRuns(int length)
    {
        var random = new Random(RandomSeed);
        var queries = new (int Left, int Right, int Threshold)[QueryCount];
        var runCount = (length + RunLength - 1) / RunLength;

        for (var i = 0; i < QueryCount; i++)
        {
            var run = random.Next(runCount);
            var runStart = run * RunLength;
            var runEnd = Math.Min(runStart + RunLength, length) - 1;
            var left = random.Next(runStart, runEnd + 1);
            var right = random.Next(left, runEnd + 1);
            queries[i] = (left, right, right - left + 1);
        }

        return queries;
    }

    [Benchmark(Baseline = true)]
    public long TallyEveryValueInRange() => ReplayQueries(_rangeTally);

    [Benchmark]
    public long PositionIndexWithBinarySearch() => ReplayQueries(_positionIndex);

    // Sums the answers rather than discarding them, so the JIT can't eliminate the
    // replay as dead code.
    private long ReplayQueries(OnlineMajorityElementInSubarraySolution.IMajorityChecker checker)
    {
        var total = 0L;

        foreach (var (left, right, threshold) in _queries)
        {
            total += checker.Query(left, right, threshold);
        }

        return total;
    }
}
