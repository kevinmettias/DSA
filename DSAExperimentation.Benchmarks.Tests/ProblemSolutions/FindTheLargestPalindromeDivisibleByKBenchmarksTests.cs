using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheLargestPalindromeDivisibleByKBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - walking the half-digit leaves largest-first
// against a digit DP over (position, remainder) states - so a harness whose arms disagree is
// timing two different problems. The class has no Setup, so DigitCount and K are the whole
// workload.
public sealed partial class FindTheLargestPalindromeDivisibleByKBenchmarksTests
{
    private const int SmallestDigitCount = 5;

    [Fact]
    public void BruteForce_SmallestDigitCount_AgreesWithDigitDpMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DigitDpMemo(), harness.BruteForce());
    }

    [Fact]
    public void DigitDpMemo_SmallestDigitCount_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.DigitDpMemo());
    }

    private static FindTheLargestPalindromeDivisibleByKBenchmarks BuildHarness() =>
        new() { DigitCount = SmallestDigitCount };
}
