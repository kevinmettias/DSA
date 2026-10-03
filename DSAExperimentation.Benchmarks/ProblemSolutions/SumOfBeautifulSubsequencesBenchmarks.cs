using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SumOfBeautifulSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfBeautifulSubsequencesSolution's, the same
// methods SumOfBeautifulSubsequencesSolutionTests proves correct. Neither strategy needs
// anything prepared beyond the array itself, so [GlobalSetup] only charges
// workload construction.
//
// Sizes are per arm. The brute force walks all 2^n subsequences, so it stops at 16;
// the divisor sieve, one Fenwick pass per divisor's multiples, runs on to LC 3671's own
// bound of 10^4. The two are compared at the sizes both run.
public class SumOfBeautifulSubsequencesBenchmarks
{
    private const int Seed = 3671;

    private Dictionary<int, int[]> _numsBySize = [];

    public static IEnumerable<int> BaselineSizes => [12, 16];

    public static IEnumerable<int> DivisorSieveSizes => [.. BaselineSizes, 1_000, 10_000];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsBySize = DivisorSieveSizes.ToDictionary(
            size => size,
            size => SumOfBeautifulSubsequencesWorkloads.BuildNums(size, Seed));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForceBitmask(int size) => SumOfBeautifulSubsequencesSolution.SumBeautyByBruteForce(_numsBySize[size]);

    [Benchmark]
    [ArgumentsSource(nameof(DivisorSieveSizes))]
    public int DivisorSieve(int size) => SumOfBeautifulSubsequencesSolution.SumBeautyByDivisorSieve(_numsBySize[size]);
}
