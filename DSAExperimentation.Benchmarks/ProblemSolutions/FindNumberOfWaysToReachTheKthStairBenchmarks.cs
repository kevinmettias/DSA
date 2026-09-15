using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindNumberOfWaysToReachTheKthStair;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindNumberOfWaysToReachTheKthStairSolution's, the
// same methods FindNumberOfWaysToReachTheKthStairTests proves correct. K stays
// in the low millions rather than up to LC's own 1e9 ceiling because the
// unmemoized arm's own call count scales ~O(k) (the k+1 prune only stops each
// branch after ~log2(k) levels of up-to-2x fanout, but that fanout revisits
// the same handful of states over and over without a cache) - 1e9 would make
// the baseline itself the bottleneck rather than the comparison. Both Params
// sit above the crossover where Memoizer's own Dictionary allocation stops
// paying for itself (below roughly K=5_000 in a local run, the unmemoized
// arm's entire O(k) walk is cheaper than one dictionary): confirmed locally at
// K=1_000_000 (~2x faster) and K=20_000_000 (~50x faster), since the memoized
// arm's own state count stays ~O(log^2 k) regardless of K, which is the actual
// effect being measured.
[MemoryDiagnoser]
public class FindNumberOfWaysToReachTheKthStairBenchmarks
{
    [Params(1_000_000, 20_000_000)]
    public int K { get; set; }

    [Benchmark(Baseline = true)]
    public int BruteRecursion() => FindNumberOfWaysToReachTheKthStairSolution.WaysByBruteRecursion(K);

    [Benchmark]
    public int MemoizedRecurrence() => FindNumberOfWaysToReachTheKthStairSolution.WaysByMemoizedRecurrence(K);
}
