using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistributeCandiesAmongChildrenISolution's, the same
// methods DistributeCandiesAmongChildrenITests proves correct. limit == candyCount
// is LC 2928's own worst case within its <=50 bound: the double loop's inner bound
// is min(candyCount - first, limit), so setting limit as high as the candy count
// keeps every iteration in range instead of an early truncation making brute force
// look artificially competitive.
[MemoryDiagnoser]
public class DistributeCandiesAmongChildrenIBenchmarks
{
    [Params(10, 50)]
    public int CandyCount { get; set; }

    [Benchmark(Baseline = true)]
    public int BruteForce() => DistributeCandiesAmongChildrenISolution.CountWaysByBruteForce(CandyCount, CandyCount);

    [Benchmark]
    public int InclusionExclusion() => DistributeCandiesAmongChildrenISolution.CountWaysByInclusionExclusion(CandyCount, CandyCount);
}
