using DSAExperimentation.LeetCode.FindTheLargestPalindromeDivisibleByK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheLargestPalindromeDivisibleByKSolution's, the
// same methods FindTheLargestPalindromeDivisibleByKSolutionTests proves correct.
//
// Sizes are per arm. The brute-force arm walks halfLength = ceil(DigitCount/2)
// half-digits, 9*10^(halfLength-1) leaves, so it stops at 9 digits; the digit-DP arm
// only pays for halfLength*K states and runs on to 1,000 digits. LC 3260 allows 10^5,
// but the memoized recursion is as deep as the half is long, so 1,000 keeps its call
// stack shallow. The two are compared at the digit counts both run.
public class FindTheLargestPalindromeDivisibleByKBenchmarks
{
    private const int K = 7;

    public static IEnumerable<int> BruteForceSizes => [5, 9];

    public static IEnumerable<int> DigitDpMemoSizes => [.. BruteForceSizes, 100, 1_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public string BruteForce(int digitCount) =>
        FindTheLargestPalindromeDivisibleByKSolution.LargestPalindromeByBruteForce(digitCount, K);

    [Benchmark]
    [ArgumentsSource(nameof(DigitDpMemoSizes))]
    public string DigitDpMemo(int digitCount) =>
        FindTheLargestPalindromeDivisibleByKSolution.LargestPalindromeByDigitDpMemo(digitCount, K);
}
