using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfEffectiveSubsequencesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - walking every subset explicitly against the OR-subset
// transform - so a harness whose arms disagree is counting the effective subsequences of two
// different arrays. Setup draws the seeded values from the fixed value domain, so the same Length
// must rebuild the same array.
//
// Both arms return an int, so they are compared directly. Neither arm mutates the array it is handed,
// so one harness serves both arms in either order.
public sealed partial class NumberOfEffectiveSubsequencesBenchmarksTests
{
    private const int SmallestLength = 12;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithOrSubsetTransform()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.OrSubsetTransform(), harness.BruteForce());
    }

    [Fact]
    public void OrSubsetTransform_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.OrSubsetTransform());
    }

    private static NumberOfEffectiveSubsequencesBenchmarks BuildHarness()
    {
        var harness = new NumberOfEffectiveSubsequencesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
