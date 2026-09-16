using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestUniqueSubarrayBenchmarks (ARCHITECTURE 17.9): both arms are
// SmallestUniqueSubarraySolution's searches for the same smallest unique window length over the
// same generated values - an exact BCL string-key bucketing against a coordinate-compressed
// rolling hash - so a harness whose arms disagree is timing two different problems. Setup is a
// pure function of Length and its own fixed seed, so the same Length must rebuild the same values.
//
// Both arms report the length itself, which is the answer LeetCode 3934 asks for. The value bound
// is what makes the agreement carry weight: with only 50 distinct values over 200 positions, no
// short window is unique, so the search runs across several candidate lengths rather than
// resolving at length 1.
public sealed partial class SmallestUniqueSubarrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_TwoHundredLowCardinalityValues_AgreesWithRollingHashBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RollingHashBinarySearch(), harness.BruteForce());
    }

    [Fact]
    public void RollingHashBinarySearch_TwoHundredLowCardinalityValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.RollingHashBinarySearch());
    }

    private static SmallestUniqueSubarrayBenchmarks BuildHarness()
    {
        var harness = new SmallestUniqueSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
