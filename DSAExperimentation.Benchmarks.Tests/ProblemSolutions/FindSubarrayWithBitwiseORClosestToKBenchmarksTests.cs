using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindSubarrayWithBitwiseORClosestToKBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the quadratic every-subarray scan against the
// list-of-distinct-ORs sweep - so a harness whose arms disagree has minimized the distance to two
// different targets. Both answers are one int, so they are compared directly.
//
// Setup's values are the same seeded draws for a given Length, so the same parameters must rebuild
// the same array and the same minimum distance. Every value is below 2^20 and the target is 2^15, so
// the distance is a real difference rather than a saturating maximum.
public sealed partial class FindSubarrayWithBitwiseORClosestToKBenchmarksTests
{
    // The smaller of Setup's [Params(80, 500)] lengths, the one whose quadratic arm is affordable.
    private const int SmallestLength = 80;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameMinimumDifference() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededValuesBelowTheTargetRange_AgreesWithOrCompression()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.OrCompression(), harness.BruteForce());
    }

    [Fact]
    public void OrCompression_SeededValuesBelowTheTargetRange_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.OrCompression());
    }

    private static FindSubarrayWithBitwiseORClosestToKBenchmarks BuildHarness()
    {
        var harness = new FindSubarrayWithBitwiseORClosestToKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
