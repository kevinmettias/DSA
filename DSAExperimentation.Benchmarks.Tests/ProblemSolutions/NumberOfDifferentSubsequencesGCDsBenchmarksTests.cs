using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfDifferentSubsequencesGCDsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - rescanning the whole array once per candidate gcd
// against walking only each candidate's multiples through this repo's own Set<int> - so a harness
// whose arms disagree is counting the distinct gcds of two different arrays. Setup draws the seeded,
// deduplicated values from the fixed value domain, so the same Length must rebuild the same array.
//
// Both arms return an int, so they are compared directly. Neither arm mutates the array it is handed,
// so one harness serves both arms in either order.
public sealed partial class NumberOfDifferentSubsequencesGCDsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CountDifferentSubsequenceGcdsByWholeArrayScan(),
            BuildHarness().CountDifferentSubsequenceGcdsByWholeArrayScan());

    [Fact]
    public void CountDifferentSubsequenceGcdsByWholeArrayScan_AgreesWithCountDifferentSubsequenceGcdsBySetMultiples()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CountDifferentSubsequenceGcdsBySetMultiples(),
            harness.CountDifferentSubsequenceGcdsByWholeArrayScan());
    }

    [Fact]
    public void CountDifferentSubsequenceGcdsBySetMultiples_AgreesWithCountDifferentSubsequenceGcdsByWholeArrayScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CountDifferentSubsequenceGcdsByWholeArrayScan(),
            harness.CountDifferentSubsequenceGcdsBySetMultiples());
    }

    private static NumberOfDifferentSubsequencesGCDsBenchmarks BuildHarness()
    {
        var harness = new NumberOfDifferentSubsequencesGCDsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
