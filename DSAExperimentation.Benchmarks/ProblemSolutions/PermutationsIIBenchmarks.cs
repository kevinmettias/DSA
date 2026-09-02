using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PermutationsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationsIISolution's, now returning the same
// permutations the test proves correct instead of merely counting them.
[MemoryDiagnoser]
public class PermutationsIIBenchmarks
{
    // Three duplicate pairs (1,1 / 2,2 / 3,3) - the input both benchmarks dedup-search over.
    private static readonly int[] ThreeDuplicatePairsInput = [1, 1, 2, 2, 3, 3];

    [Benchmark(Baseline = true)]
    public List<List<int>> SpecializedRecursive() =>
        PermutationsIISolution.PermuteUniqueBySpecializedRecursion(ThreeDuplicatePairsInput);

    [Benchmark]
    public List<List<int>> Backtracking() =>
        PermutationsIISolution.PermuteUniqueByBacktracking(ThreeDuplicatePairsInput);
}
