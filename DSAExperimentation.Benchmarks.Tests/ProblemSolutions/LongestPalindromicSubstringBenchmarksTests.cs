using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestPalindromicSubstringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - expanding around every center against this repo's
// linear Manacher pass - so a harness whose arms disagree is timing two different problems. Both
// arms return the palindrome itself, compared directly. Setup draws the text from a fixed seed
// over a tiny alphabet, which is what makes expansions run long, and the same Length must rebuild
// the same text.
public sealed partial class LongestPalindromicSubstringBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().Manacher(),
            BuildHarness().Manacher());

    [Fact]
    public void ExpandAroundCenter_SmallestLength_AgreesWithManacher()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Manacher(), harness.ExpandAroundCenter());
    }

    [Fact]
    public void Manacher_SmallestLength_AgreesWithExpandAroundCenter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ExpandAroundCenter(), harness.Manacher());
    }

    private static LongestPalindromicSubstringBenchmarks BuildHarness()
    {
        var harness = new LongestPalindromicSubstringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
