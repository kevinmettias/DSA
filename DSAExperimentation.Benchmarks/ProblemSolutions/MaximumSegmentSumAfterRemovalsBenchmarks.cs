using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSegmentSumAfterRemovals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSegmentSumAfterRemovalsSolution's, the same
// methods MaximumSegmentSumAfterRemovalsTests proves correct. The removal order is
// a full shuffled permutation of the index range so neither arm gets a degenerate
// suffix-first order that would keep every merge on one end; summing the returned
// answer[] forces a full pass rather than one index's worth of work.
[MemoryDiagnoser]
public class MaximumSegmentSumAfterRemovalsBenchmarks
{
    private const int RandomSeed = 2382; // LC problem number
    private const int MaxValueExclusive = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int[] _removeQueries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _removeQueries = Shuffle(random, Enumerable.Range(0, Length).ToArray());
    }

    private static int[] Shuffle(Random random, int[] values)
    {
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }

    [Benchmark(Baseline = true)]
    public long RescanAfterEachRemoval() =>
        MaximumSegmentSumAfterRemovalsSolution
            .MaximumSegmentSumsByRescanAfterEachRemoval(_nums, _removeQueries).Sum();

    [Benchmark]
    public long ReverseTimeDisjointSet() =>
        MaximumSegmentSumAfterRemovalsSolution
            .MaximumSegmentSumsByReverseTimeDisjointSet(_nums, _removeQueries).Sum();
}
