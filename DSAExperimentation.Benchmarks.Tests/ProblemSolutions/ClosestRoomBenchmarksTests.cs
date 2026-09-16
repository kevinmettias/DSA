using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ClosestRoomBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a per-query linear scan against a descending sweep over rooms
// and queries plus a binary search over the eligible ids - so a harness whose arms disagree is
// timing two different problems. Setup draws the rooms and the query script from one fixed seed, so
// the same RoomCount must rebuild the same rooms and the same script; otherwise two published
// numbers were never comparable in the first place.
//
// AnswerText.Of, not OfUnorderedSet: the answers come back one per query in query order, and a set
// rendering would score an answer against the wrong query.
public sealed partial class ClosestRoomBenchmarksTests
{
    private const int SmallestRoomCount = 50;

    [Fact]
    public void Setup_SameRoomCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PerQueryScan()),
            AnswerText.Of(BuildHarness().PerQueryScan()));

    [Fact]
    public void PerQueryScan_SameRoomsAndQueryScript_AgreesWithSortedSweepWithBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SortedSweepWithBinarySearch()),
            AnswerText.Of(harness.PerQueryScan()));
    }

    [Fact]
    public void SortedSweepWithBinarySearch_SameRoomsAndQueryScript_AgreesWithPerQueryScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PerQueryScan()),
            AnswerText.Of(harness.SortedSweepWithBinarySearch()));
    }

    private static ClosestRoomBenchmarks BuildHarness()
    {
        var harness = new ClosestRoomBenchmarks { RoomCount = SmallestRoomCount };
        harness.Setup();

        return harness;
    }
}
