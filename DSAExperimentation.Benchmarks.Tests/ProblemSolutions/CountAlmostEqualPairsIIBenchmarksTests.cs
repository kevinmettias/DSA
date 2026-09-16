using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountAlmostEqualPairsIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - materializing every one- and two-swap result and
// comparing, against the same bounded reachability check driven through Backtrack.TrySearch - so a
// harness whose arms disagree is timing two different problems. Both arms return a long, so they are
// compared directly. Setup draws from one fixed seed and charges the zero-padding to itself, so the
// same Length must rebuild the same padded values, and its documented shape is that those values are
// drawn across LC 3267's whole value range: no two random seven-digit numbers land within the swap
// budget, so both arms scan every pair without ever reaching a match.
public sealed partial class CountAlmostEqualPairsIIBenchmarksTests
{
    private const int SmallestLength = 10;

    // Random seven-digit values are never within two digit swaps of one another.
    private const long ExpectedMatchingPairs = 0L;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameUnmatchingWorkload()
    {
        Assert.Equal(
            BuildHarness().BoundedSwapBruteForce(),
            BuildHarness().BoundedSwapBruteForce());

        Assert.Equal(ExpectedMatchingPairs, BuildHarness().BoundedSwapBacktrack());
    }

    [Fact]
    public void BoundedSwapBruteForce_TenRandomValues_AgreesWithBoundedSwapBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BoundedSwapBacktrack(), harness.BoundedSwapBruteForce());
    }

    [Fact]
    public void BoundedSwapBacktrack_TenRandomValues_AgreesWithBoundedSwapBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BoundedSwapBruteForce(), harness.BoundedSwapBacktrack());
    }

    private static CountAlmostEqualPairsIIBenchmarks BuildHarness()
    {
        var harness = new CountAlmostEqualPairsIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
