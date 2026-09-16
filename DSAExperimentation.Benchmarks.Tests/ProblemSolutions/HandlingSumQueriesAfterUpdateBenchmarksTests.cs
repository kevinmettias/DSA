using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HandlingSumQueriesAfterUpdateBenchmarks (ARCHITECTURE 17.9): both arms are
// HandlingSumQueriesAfterUpdateSolution's - the mutable bit array rescanned per query against this
// repo's LazySegmentTree with FlipCountOperation - so a harness whose arms disagree is timing two
// different problems. Both arms answer with LeetCode 2569's real shape, the long[] of type-3
// readings in query order, so AnswerText.Of compares them reading by reading. Each arm works on
// state it owns for the length of the call (the rescan arm clones nums1, the tree arm builds its
// own LazySegmentTree over nums1), so one harness is safe to call twice in either order and arm
// order does not matter. Setup draws nums1, nums2 and the mixed query stream off one seed, so the
// same Length must rebuild all three.
public sealed partial class HandlingSumQueriesAfterUpdateBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ArrayRescan()),
            AnswerText.Of(BuildHarness().ArrayRescan()));

    [Fact]
    public void ArrayRescan_MixedQueryStream_AgreesWithLazySegmentTreeFlip()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.LazySegmentTreeFlip()), AnswerText.Of(harness.ArrayRescan()));
    }

    [Fact]
    public void LazySegmentTreeFlip_MixedQueryStream_AgreesWithArrayRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ArrayRescan()), AnswerText.Of(harness.LazySegmentTreeFlip()));
    }

    private static HandlingSumQueriesAfterUpdateBenchmarks BuildHarness()
    {
        var harness = new HandlingSumQueriesAfterUpdateBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
