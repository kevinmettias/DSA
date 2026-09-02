using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountTheNumberOfArraysWithKMatchingAdjacentElements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution's, the same
// methods CountTheNumberOfArraysWithKMatchingAdjacentElementsTests proves
// correct. m stays fixed at 2 and k at half of n-1 so the brute-force arm's
// 2^n enumeration stays finishable while ArrayLength grows.
[MemoryDiagnoser]
public class CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarks
{
    private const int AlphabetSize = 2;

    [Params(10, 18)]
    public int ArrayLength;

    private int _matchCount;

    [GlobalSetup]
    public void Setup() => _matchCount = (ArrayLength - 1) / 2;

    [Benchmark(Baseline = true)]
    public long BruteForce() =>
        CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution.CountGoodArraysByBruteForce(ArrayLength, AlphabetSize, _matchCount);

    [Benchmark]
    public long ModularCombinatorics() =>
        CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution.CountGoodArraysByModularCombinatorics(ArrayLength, AlphabetSize, _matchCount);
}
