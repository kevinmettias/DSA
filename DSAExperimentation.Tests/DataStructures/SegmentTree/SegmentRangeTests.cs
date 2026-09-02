using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed class SegmentRangeTests
{
    [Fact]
    public void Split_EvenSpan_HalvesItIntoTwoEqualParts()
    {
        var (left, right) = new SegmentRange(1, 0, 3).Split();

        Assert.Equal((0, 1), (left.Start, left.End));
        Assert.Equal((2, 3), (right.Start, right.End));
    }

    [Fact]
    public void Split_OddSpan_GivesTheExtraElementToTheLeftHalf()
    {
        var (left, right) = new SegmentRange(1, 0, 4).Split();

        Assert.Equal((0, 2), (left.Start, left.End));
        Assert.Equal((3, 4), (right.Start, right.End));
    }

    [Fact]
    public void Split_ProducesTheHeapChildIndicesOfTheParentNode()
    {
        var (left, right) = new SegmentRange(3, 0, 3).Split();

        Assert.Equal(7, left.Node);
        Assert.Equal(8, right.Node);
    }

    [Fact]
    public void Split_CoversTheParentSpanWithoutOverlapOrGap()
    {
        var parent = new SegmentRange(1, 5, 12);

        var (left, right) = parent.Split();

        Assert.Equal(parent.Start, left.Start);
        Assert.Equal(parent.End, right.End);
        Assert.Equal(left.End + 1, right.Start);
    }

    [Fact]
    public void Split_TwoElementSpan_YieldsTwoSingletons()
    {
        var (left, right) = new SegmentRange(1, 7, 8).Split();

        Assert.Equal((7, 7), (left.Start, left.End));
        Assert.Equal((8, 8), (right.Start, right.End));
    }
}
