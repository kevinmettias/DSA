using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfLongestIncreasingSubsequenceBenchmarks (ARCHITECTURE 17.9): both arms
// count the longest increasing subsequences of the same seeded array - the textbook O(n^2) DP against
// the SegmentTree arm keyed by coordinate-compressed ranks - so a harness whose arms disagree is
// timing two different questions. The count is the problem's whole answer rather than a proxy of it.
// Setup draws that array from a fixed seed, so the same Length must rebuild the same one.
public sealed partial class NumberOfLongestIncreasingSubsequenceBenchmarksTests
{
    private const int SmallestLength = 3_000;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DynamicProgramming(), BuildHarness().DynamicProgramming());

    [Fact]
    public void DynamicProgramming_AgreesWithSegmentTreeCoordinateCompression()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeCoordinateCompression(), harness.DynamicProgramming());
    }

    [Fact]
    public void SegmentTreeCoordinateCompression_AgreesWithDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicProgramming(), harness.SegmentTreeCoordinateCompression());
    }

    private static NumberOfLongestIncreasingSubsequenceBenchmarks BuildHarness()
    {
        var harness = new NumberOfLongestIncreasingSubsequenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
