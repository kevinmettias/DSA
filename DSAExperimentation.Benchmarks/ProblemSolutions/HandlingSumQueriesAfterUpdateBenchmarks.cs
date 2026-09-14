using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HandlingSumQueriesAfterUpdate;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HandlingSumQueriesAfterUpdateSolution's, the same
// methods HandlingSumQueriesAfterUpdateTests proves correct - re-scanning a mutable
// bit array on every flip and every "add p per current one" query (baseline, the
// naive approach the problem is designed to make too slow) vs. this repo's own
// LazySegmentTree<int, bool, FlipCountOperation> answering both the range flip and
// the current-one-count as O(log n) operations. [GlobalSetup] builds nums1, nums2 and
// the query stream - already LeetCode's own input shape, so no hoisted overload is
// needed - and query kinds cycle through flip/add/read so both strategies pay a
// representative mixed workload instead of one query kind dominating.
[MemoryDiagnoser]
public class HandlingSumQueriesAfterUpdateBenchmarks
{
    private const int RandomSeed = 2569; // LeetCode problem number
    private const int MaxNums2ValueExclusive = 1_000;
    private const int QueryKindModulus = 3;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(0, 2)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxNums2ValueExclusive)).ToArray();
        _queries = Enumerable.Range(0, Length).Select(i => BuildQuery(random, i)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long[] ArrayRescan() =>
        HandlingSumQueriesAfterUpdateSolution.HandleQueryByArrayRescan(_nums1, _nums2, _queries);

    [Benchmark]
    public long[] LazySegmentTreeFlip() =>
        HandlingSumQueriesAfterUpdateSolution.HandleQueryByLazySegmentTree(_nums1, _nums2, _queries);

    private int[] BuildQuery(Random random, int index)
    {
        var kind = index % QueryKindModulus;

        if (kind == 0)
        {
            var left = random.Next(0, Length);
            var right = random.Next(left, Length);
            return [1, left, right];
        }

        return kind == 1 ? [2, random.Next(1, MaxNums2ValueExclusive), 0] : [3, 0, 0];
    }
}
