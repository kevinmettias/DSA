using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FairDistributionOfCookies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FairDistributionOfCookiesSolution's, the same methods
// FairDistributionOfCookiesTests proves correct - the hand-rolled recursion against
// this repo's generic Backtrack.Search closed over identical steps, the same
// hand-rolled-vs-generic-primitive shape PartitionToKEqualSumSubsetsBenchmarks uses.
//
// Bags are random in [1, MaxBagSize) so a real search is needed instead of an
// immediately-degenerate all-equal split. The workload is LeetCode's own input shape
// already, so there is nothing for a hoisted overload to prepare: [GlobalSetup]
// hands both arms the same int[].
[MemoryDiagnoser]
public class FairDistributionOfCookiesBenchmarks
{
    private const int RandomSeed = 2305; // LC problem number
    private const int MaxBagSize = 20;
    private const int Children = 3;

    private int[] _cookies = [];

    [Params(6, 8)]
    public int BagCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _cookies = Enumerable.Range(0, BagCount).Select(_ => random.Next(1, MaxBagSize)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RecursiveBacktracking() =>
        FairDistributionOfCookiesSolution.DistributeCookiesByRecursiveBacktracking(_cookies, Children);

    [Benchmark]
    public int BacktrackPrimitive() =>
        FairDistributionOfCookiesSolution.DistributeCookiesByBacktrackSearch(_cookies, Children);
}
