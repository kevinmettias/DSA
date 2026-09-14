using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestIncreasingSubsequenceII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestIncreasingSubsequenceIISolution's, the same
// methods LongestIncreasingSubsequenceIITests proves correct - the textbook O(n^2) DP
// against this repo's own SegmentTree<int, MaxOperation<int>> keyed directly by value,
// which replaces the inner rescan with one O(log maxValue) range-max query per
// element. [GlobalSetup] owns the workload construction.
[MemoryDiagnoser]
public class LongestIncreasingSubsequenceIIBenchmarks
{
    private const int RandomSeed = 2407; // LC problem number
    private const int K = 5;

    [Params(2_000, 6_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, Length + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgramming()
        => LongestIncreasingSubsequenceIISolution.LengthOfLisByDynamicProgramming(_values, K);

    [Benchmark]
    public int SegmentTreeValueWindow()
        => LongestIncreasingSubsequenceIISolution.LengthOfLisBySegmentTreeValueWindow(_values, K);
}
