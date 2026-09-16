using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MissingNumberBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - the brute-force scan against the set-membership pass - so a harness whose arms
// disagree is timing two different problems. Both arms only read the value array built in [GlobalSetup], so
// one harness instance is safe to call twice in either order. Setup draws that array from one fixed seed, so
// the same Length must rebuild the same array; otherwise two published numbers were never comparable.
public sealed partial class MissingNumberBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededValueRange_AgreesWithSetMembership()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SetMembership(), harness.BruteForce());
    }

    [Fact]
    public void SetMembership_SeededValueRange_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SetMembership());
    }

    private static MissingNumberBenchmarks BuildHarness()
    {
        var harness = new MissingNumberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
