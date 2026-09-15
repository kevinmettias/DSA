using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShoppingOffers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShoppingOffersSolution's, the same methods
// ShoppingOffersTests proves correct. Three pairwise offers over the same three
// items give multiple offer-application orders that land on the same
// remaining-needs vector - exactly the redundant recomputation BruteForce pays for
// and MemoizedDfs avoids.
[MemoryDiagnoser]
public class ShoppingOffersBenchmarks
{
    private static readonly int[] Price = [2, 3, 4];

    private static readonly int[][] Special =
    [
        [1, 1, 0, Price[0] + Price[1] - 1],
        [0, 1, 1, Price[1] + Price[2] - 1],
        [1, 0, 1, Price[0] + Price[2] - 1],
    ];

    private int[] _needs = [];

    [Params(4, 7)]
    public int NeedsPerItem { get; set; }

    [GlobalSetup]
    public void Setup() => _needs = [NeedsPerItem, NeedsPerItem, NeedsPerItem];

    [Benchmark(Baseline = true)]
    public int BruteForce() => ShoppingOffersSolution.MinCostByBruteForce(Price, Special, _needs);

    [Benchmark]
    public int MemoizedDfs() => ShoppingOffersSolution.MinCostByMemoizedDfs(Price, Special, _needs);
}
