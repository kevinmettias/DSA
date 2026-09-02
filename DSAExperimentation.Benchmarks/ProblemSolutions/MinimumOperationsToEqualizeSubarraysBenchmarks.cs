using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.LeetCode.MinimumOperationsToEqualizeSubarrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumOperationsToEqualizeSubarraysSolution's,
// the same methods MinimumOperationsToEqualizeSubarraysTests proves correct.
// The merge-sort tree and run-id index are built once via the solution's own
// BuildIndex and charged to [GlobalSetup] through the MergeSortTree arm's
// hoisted overload, so only the per-query answering is measured - the same
// split OpenTheLockBenchmarks uses for LockGraph.Build.
[MemoryDiagnoser]
public class MinimumOperationsToEqualizeSubarraysBenchmarks
{
    private const int Seed = 3762;
    private const int K = 4;
    private const int MaxMultiplesPerElement = 1_000;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;
    private int[][] _queries = null!;
    private int[] _runId = null!;
    private SegmentTree<int[], SortedMergeOperation> _tree = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);

        // Every element stays on the same remainder mod k, so every query
        // resolves to a real answer rather than -1 - the workload measures
        // the median/deviation computation itself, not the -1 short circuit.
        _nums = Enumerable.Range(0, Length)
            .Select(_ => 1 + (random.Next(MaxMultiplesPerElement) * K))
            .ToArray();

        _queries = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var left = random.Next(Length);
                var right = left + random.Next(Length - left);
                return new[] { left, right };
            })
            .ToArray();

        (_tree, _runId) = MinimumOperationsToEqualizeSubarraysSolution.BuildIndex(_nums, K);
    }

    [Benchmark(Baseline = true)]
    public long[] BruteForce() =>
        MinimumOperationsToEqualizeSubarraysSolution.MinOperationsByBruteForce(_nums, _queries, K);

    [Benchmark]
    public long[] MergeSortTree() =>
        MinimumOperationsToEqualizeSubarraysSolution.MinOperationsByMergeSortTree(_tree, _runId, _queries, K);
}
