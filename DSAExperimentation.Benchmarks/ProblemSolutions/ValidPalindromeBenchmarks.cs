using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ValidPalindrome;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidPalindromeSolution's, the same methods
// ValidPalindromeSolutionTests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (`Baseline() => 1`, `PrimitiveComposed() => 1`)
// rather than a second strategy to reconcile. The operand comes from
// ValidPalindromeWorkloads: punctuation and casing noise around a seeded true
// palindrome, in the shape of LeetCode 125's own first example, so the two-pointer
// scan has to meet in the middle and the normalized copy has to read every
// character. Length stops at LC 125's 200,000-character cap.
public class ValidPalindromeBenchmarks
{
    private const int RandomSeed = 125; // LC problem number

    private string _value = "";

    [Params(2_000, 20_000, 200_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _value = ValidPalindromeWorkloads.BuildNoisyPalindrome(Length, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public bool IsPalindromeByTwoPointerScan() => ValidPalindromeSolution.IsPalindromeByTwoPointerScan(_value);

    [Benchmark]
    public bool NormalizedReversal() => ValidPalindromeSolution.IsPalindromeByNormalizedReversal(_value);
}
