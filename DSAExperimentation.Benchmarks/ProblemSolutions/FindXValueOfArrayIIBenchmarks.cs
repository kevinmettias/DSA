using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindXValueOfArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindXValueOfArrayIISolution's, the same methods
// FindXValueOfArrayIITests proves correct. k is fixed at the maximum LeetCode
// allows (5), the segment tree's least favorable case since every node's Counts
// matrix is 5x5 - the workload choice that actually stresses the strategy this
// benchmark exists to justify.
[MemoryDiagnoser]
public class FindXValueOfArrayIIBenchmarks
{
    private const int RandomSeed = 3525; // LeetCode problem number
    private const int Modulus = 5;
    private const int QueryCount = 200;
    private const int ValueUpperBound = 1_000_000_000;

    [Params(1_000, 20_000)]
    public int Length;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _nums[i] = random.Next(1, ValueUpperBound);
        }

        _queries = new int[QueryCount][];

        for (var i = 0; i < QueryCount; i++)
        {
            _queries[i] =
            [
                random.Next(Length),
                random.Next(1, ValueUpperBound),
                random.Next(Length),
                random.Next(Modulus),
            ];
        }
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => FindXValueOfArrayIISolution.XValueCountsByBruteForce(_nums, Modulus, _queries);

    [Benchmark]
    public int[] SegmentTreeAutomaton() => FindXValueOfArrayIISolution.XValueCountsBySegmentTreeAutomaton(_nums, Modulus, _queries);
}
