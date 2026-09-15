using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistributeCandiesAmongChildrenISolution's, the same
// methods DistributeCandiesAmongChildrenITests proves correct. limit == n is LC
// 2928's own worst case within its <=50 bound: the double loop's inner bound is
// min(n - first, limit), so setting limit as high as n keeps every iteration in
// range instead of an early truncation making brute force look artificially
// competitive.
[MemoryDiagnoser]
public class DistributeCandiesAmongChildrenIBenchmarks
{
    [Params(10, 50)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public int BruteForce() => DistributeCandiesAmongChildrenISolution.CountWaysByBruteForce(N, N);

    [Benchmark]
    public int InclusionExclusion() => DistributeCandiesAmongChildrenISolution.CountWaysByInclusionExclusion(N, N);
}
