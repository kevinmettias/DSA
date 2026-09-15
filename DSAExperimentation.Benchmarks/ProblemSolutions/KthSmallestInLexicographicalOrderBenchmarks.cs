using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.KthSmallestInLexicographicalOrder;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthSmallestInLexicographicalOrderSolution's, the same
// methods KthSmallestInLexicographicalOrderTests proves correct. Generating every
// number 1..n as a string and sorting it (O(n log n)) vs. this repo's own
// successor-function DepthFirstSearch.Traverse walking the implicit 10-ary tree
// directly in lexicographical order (O(n), no sort needed) - the same primitive
// LexicographicalNumbersTests (LC 386) already uses, just indexed to the k-th
// element instead of returning the whole order. Traverse's per-node Stack/HashSet
// bookkeeping carries a real constant-factor cost - a dry run showed it losing to
// the sort-based baseline by 2-5x at N=2_000/20_000 - but O(n) overtakes O(n log n)
// as n grows: roughly even by N=200_000, clearly ahead (~2x) by N=2_000_000. Large
// [Params] values, closer to this problem's LeetCode input range (n up to 10^9),
// are what it takes to actually see that crossover.
[MemoryDiagnoser]
public class KthSmallestInLexicographicalOrderBenchmarks
{

    private int _k;

    [Params(200_000, 2_000_000)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup() => _k = N / AlgorithmConstants.HalvingFactor;

    [Benchmark(Baseline = true)]
    public int GenerateAndSortStrings() =>
        KthSmallestInLexicographicalOrderSolution.FindKthNumberByGenerateAndSort(N, _k);

    [Benchmark]
    public int DepthFirstTraversalOrder() =>
        KthSmallestInLexicographicalOrderSolution.FindKthNumberByDepthFirstTraversal(N, _k);
}
