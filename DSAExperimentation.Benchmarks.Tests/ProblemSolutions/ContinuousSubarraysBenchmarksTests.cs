using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContinuousSubarraysBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n^2) rescan from every starting index against the two
// monotonic deque windows - so a harness whose arms disagree is timing two different problems. Both
// arms return a long, so they are compared directly. Setup draws from one fixed seed, so the same
// Length must rebuild the same array, and its documented shape is that narrow values keep windows
// long: every single-element subarray has spread zero, which the problem's limit of 2 admits, so no
// admissible answer can ever fall below the number of starting points.
public sealed partial class ContinuousSubarraysBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWideWindowWorkload()
    {
        Assert.Equal(
            BuildHarness().BruteForceAllStartingPoints(),
            BuildHarness().BruteForceAllStartingPoints());

        Assert.True(BuildHarness().DoubleMonotonicDeque() >= SmallestLength);
    }

    [Fact]
    public void BruteForceAllStartingPoints_NarrowValues_AgreesWithDoubleMonotonicDeque()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DoubleMonotonicDeque(), harness.BruteForceAllStartingPoints());
    }

    [Fact]
    public void DoubleMonotonicDeque_NarrowValues_AgreesWithBruteForceAllStartingPoints()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllStartingPoints(), harness.DoubleMonotonicDeque());
    }

    private static ContinuousSubarraysBenchmarks BuildHarness()
    {
        var harness = new ContinuousSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
