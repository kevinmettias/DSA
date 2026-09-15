using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumBalancedSubsequenceSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumBalancedSubsequenceSumSolution's, the same
// methods MaximumBalancedSubsequenceSumTests proves correct. Mirrors
// CountOfSmallerNumbersAfterSelfBenchmarks' shape - the textbook O(n^2) pairwise
// scan against an O(n log n) sweep through a repo index structure, this time a
// SegmentTree<long,MaxOperation<long>> instead of a FenwickTree<int,SumOperation<int>>.
// The crossover lands later than CountOfSmallerNumbersAfterSelfBenchmarks' own -
// SegmentTree's recursive per-call overhead is real, so BruteForce's tight array
// scan still wins at 2,000 - which is why 20,000 is included too: large enough for
// SegmentTreeSweep's better asymptotics to actually pay for that overhead.
[MemoryDiagnoser]
public class MaximumBalancedSubsequenceSumBenchmarks
{
    private const int RandomSeed = 2926; // LeetCode problem number
    private const int ValueBound = 1_000;

    private int[] _nums = [];

    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => MaximumBalancedSubsequenceSumSolution.MaxBalancedSumByBruteForce(_nums);

    [Benchmark]
    public long SegmentTreeSweep() => MaximumBalancedSubsequenceSumSolution.MaxBalancedSumBySegmentTree(_nums);
}
