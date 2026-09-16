using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestNiceSubstringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning a presence table for every substring
// against recursing on the first character missing its opposite-case partner - so a harness whose
// arms disagree is timing two different problems. Both arms return the substring itself, compared
// directly. Setup draws from a fixed seed over a tiny mixed-case alphabet, which keeps both
// variants paying real work, and the same Length must rebuild the same text.
public sealed partial class LongestNiceSubstringBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().DivideAndConquer(),
            BuildHarness().DivideAndConquer());

    [Fact]
    public void BruteForceAllSubstrings_SmallestLength_AgreesWithDivideAndConquer()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DivideAndConquer(), harness.BruteForceAllSubstrings());
    }

    [Fact]
    public void DivideAndConquer_SmallestLength_AgreesWithBruteForceAllSubstrings()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllSubstrings(), harness.DivideAndConquer());
    }

    private static LongestNiceSubstringBenchmarks BuildHarness()
    {
        var harness = new LongestNiceSubstringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
