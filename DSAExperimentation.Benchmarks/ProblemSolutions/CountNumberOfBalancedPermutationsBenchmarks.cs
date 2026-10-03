using DSAExperimentation.LeetCode.CountNumberOfBalancedPermutations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountNumberOfBalancedPermutationsSolution's, the
// same methods CountNumberOfBalancedPermutationsSolutionTests proves correct
// (CountAnagramsBenchmarks precedent). Digits are drawn from a small alphabet so
// repeats are common, exercising the DP's inverse-factorial division path
// instead of degenerating to every digit distinct.
//
// Sizes are per arm. Brute force enumerates distinct permutations and would not
// finish past 9 digits, so it stops there; the digit-count DP runs on to LC 3343's
// own bound of 80 digits, and the two are compared at the lengths both run.
public class CountNumberOfBalancedPermutationsBenchmarks
{
    private const int Seed = 3343; // LC problem number
    private const string Digits = "01234";

    private Dictionary<int, string> _numByLength = [];

    public static IEnumerable<int> BruteForceSizes => [6, 9];

    public static IEnumerable<int> DigitCountDpSizes => [.. BruteForceSizes, 20, 80];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _numByLength = DigitCountDpSizes.ToDictionary(length => length, BuildNum);

    private static string BuildNum(int length)
    {
        var random = new Random(Seed);

        return new string(Enumerable.Range(0, length).Select(_ => Digits[random.Next(Digits.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForce(int length) =>
        CountNumberOfBalancedPermutationsSolution.CountBalancedPermutationsByBruteForce(_numByLength[length]);

    [Benchmark]
    [ArgumentsSource(nameof(DigitCountDpSizes))]
    public long DigitCountDp(int length) =>
        CountNumberOfBalancedPermutationsSolution.CountBalancedPermutationsByDigitCountDp(_numByLength[length]);
}
