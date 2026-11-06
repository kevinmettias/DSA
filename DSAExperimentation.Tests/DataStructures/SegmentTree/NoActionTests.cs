using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

// The trivial action's laws, checked although no tree calls it: its one update is the identity, it
// composes with itself into itself, and applying it leaves a range's value as it was, over any range
// and for an element of any type.
public sealed partial class NoActionTests
{
    private const int Aggregate = 17;
    private const int AnyRangeLength = 9;
    private const string ReferenceAggregate = "seventeen";

    [Fact]
    public void NoUpdate_IsTheOneValueOfItsType() => Assert.Equal(default, NoAction<int>.NoUpdate);

    [Fact]
    public void ComposeUpdate_OfNoUpdates_IsNoUpdate() =>
        Assert.Equal(
            NoAction<int>.NoUpdate,
            NoAction<int>.ComposeUpdate(NoAction<int>.NoUpdate, NoAction<int>.NoUpdate));

    [Fact]
    public void ApplyUpdate_AnyRange_LeavesTheAggregateAsItWas() =>
        Assert.Equal(Aggregate, NoAction<int>.ApplyUpdate(Aggregate, NoAction<int>.NoUpdate, AnyRangeLength));

    [Fact]
    public void ApplyUpdate_ReferenceElement_ReturnsTheSameInstance() =>
        Assert.Same(
            ReferenceAggregate,
            NoAction<string>.ApplyUpdate(ReferenceAggregate, NoAction<string>.NoUpdate, AnyRangeLength));
}
