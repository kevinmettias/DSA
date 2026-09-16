using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithFixedBoundsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescoring every subarray's own min/max from scratch
// against the build-once segment-tree pair - so a harness whose arms disagree is timing two different
// problems, not two ways of answering one. Setup draws the array from one fixed seed, so the same
// Length must rebuild the same array; otherwise two published numbers were never comparable in the
// first place.
//
// The generated array is private and the subarray count is the only thing either arm reports, so the
// documented shape is asserted through that: the answer counts subarrays of a Length-long array, so
// it can never exceed Length * (Length + 1) / 2.
public sealed partial class CountSubarraysWithFixedBoundsBenchmarksTests
{
    private const int SmallestLength = 20;

    private const long SubarrayCount = (long)SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArray()
    {
        Assert.InRange(BuildHarness().RescanEachSubarray(), 0, SubarrayCount);
        Assert.Equal(BuildHarness().RescanEachSubarray(), BuildHarness().RescanEachSubarray());
    }

    [Fact]
    public void RescanEachSubarray_SeededArrayWithMinTwoMaxEight_AgreesWithSegmentTreeRangeQueries()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeRangeQueries(), harness.RescanEachSubarray());
    }

    [Fact]
    public void SegmentTreeRangeQueries_SeededArrayWithMinTwoMaxEight_AgreesWithRescanEachSubarray()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RescanEachSubarray(), harness.SegmentTreeRangeQueries());
    }

    private static CountSubarraysWithFixedBoundsBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithFixedBoundsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
