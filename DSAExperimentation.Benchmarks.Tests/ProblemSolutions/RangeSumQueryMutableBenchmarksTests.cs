using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RangeSumQueryMutableBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the sum of every SumRange call in one interleaved
// update/sumRange stream - so a harness whose arms disagree is timing two different problems. Both
// arms sum the returned range sums into one running total, so the comparison is a direct scalar
// one. Each arm's factory builds a fresh instance inside the call, so the updates one arm applies
// never leak into the other and one harness is safe to call twice in either order.
//
// Setup draws the initial array and the operation stream from one fixed seed, so the same Length
// must rebuild the same pair.
public sealed partial class RangeSumQueryMutableBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ArrayRescan(), BuildHarness().ArrayRescan());

    [Fact]
    public void ArrayRescan_SeededOperationStream_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayRescan(), harness.SegmentTreeQuery());
    }

    [Fact]
    public void SegmentTreeQuery_SeededOperationStream_AgreesWithTheArrayArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeQuery(), harness.ArrayRescan());
    }

    private static RangeSumQueryMutableBenchmarks BuildHarness()
    {
        var harness = new RangeSumQueryMutableBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
