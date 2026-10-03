using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfBalancedPermutationsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - enumerating distinct permutations against the
// per-digit split DP - so a harness whose arms disagree is timing two different problems. Setup
// seeds num from a fixed random draw over a small digit alphabet, so the same Length must rebuild
// the same num.
public sealed partial class CountNumberOfBalancedPermutationsBenchmarksTests
{
    private const int SmallestLength = 6;

    // Distinct permutations of a six-digit num, the ceiling on how many of them can be balanced.
    private const long MostSixDigitPermutations = 720;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: num holds SmallestLength digits, so the answer can never exceed
        // the SmallestLength! distinct permutations they can be arranged into.
        Assert.InRange(first.BruteForce(SmallestLength), 0L, MostSixDigitPermutations);
        Assert.Equal(first.BruteForce(SmallestLength), second.BruteForce(SmallestLength));
    }

    [Fact]
    public void BruteForce_SmallDigitAlphabet_AgreesWithDigitCountDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DigitCountDp(SmallestLength), harness.BruteForce(SmallestLength));
    }

    [Fact]
    public void DigitCountDp_SmallDigitAlphabet_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(SmallestLength), harness.DigitCountDp(SmallestLength));
    }

    private static CountNumberOfBalancedPermutationsBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfBalancedPermutationsBenchmarks();
        harness.Setup();

        return harness;
    }
}
