using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromicSubstringsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one count - expanding around every centre against Manacher's linear scan
// - so a harness whose arms disagree is timing two different problems. Setup draws the text from one
// seeded Random over the fixture's four-letter alphabet, so the same Length must rebuild the same
// text; the smallest tuned Length keeps the quadratic arm's expansions affordable.
public sealed partial class PalindromicSubstringsBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().Manacher(), BuildHarness().Manacher());

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

    private static PalindromicSubstringsBenchmarks BuildHarness()
    {
        var harness = new PalindromicSubstringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
