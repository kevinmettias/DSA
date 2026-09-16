using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.Tests.Algorithms.Searching;

// Each position of Repeating is distinguished by at least one relation: the two 2s at 1 and 3
// make strict and or-equal disagree on both sides - a strict sweep steps past an equal
// neighbour, an or-equal sweep stops on it - and the three 4s make each relation's "there is
// no such position anywhere" case reachable without any position having to be a global
// extreme that would hide the sweep's tie handling behind arithmetic instead.
//
// The two sentinels are the ones the call sites in problem solutions reach for: -1 for a
// boundary that would have to be to the left of the first position, and one past the last
// position for a boundary to the right, so a range reads as [boundary + 1, position] with no
// special case for either end. Neither is baked into the sweep; both are passed per call.
public sealed partial class NearestBoundaryTests
{
    private static readonly int[] Repeating = [4, 2, 4, 2, 4];

    private const int NoPositionToTheLeft = -1;
    private const int PastTheLastPosition = 5;
    private const int SomeOtherSentinel = 77;

    [Fact]
    public void SmallerToTheLeft_RepeatingValues_StopsAtTheNearestStrictlySmallerPosition()
    {
        var boundaries = NearestBoundary.SmallerToTheLeft(Repeating, NoPositionToTheLeft);

        Assert.Equal([-1, -1, 1, -1, 3], boundaries);
    }

    [Fact]
    public void SmallerOrEqualToTheLeft_RepeatingValues_StopsAtAnEqualNeighbour()
    {
        var boundaries = NearestBoundary.SmallerOrEqualToTheLeft(Repeating, NoPositionToTheLeft);

        Assert.Equal([-1, -1, 1, 1, 3], boundaries);
    }

    [Fact]
    public void GreaterToTheLeft_RepeatingValues_ReportsNoneWhereAnEqualValueBlocksTheWay()
    {
        var boundaries = NearestBoundary.GreaterToTheLeft(Repeating, NoPositionToTheLeft);

        Assert.Equal([-1, 0, -1, 2, -1], boundaries);
    }

    [Fact]
    public void GreaterOrEqualToTheLeft_RepeatingValues_StopsAtAnEqualNeighbour()
    {
        var boundaries = NearestBoundary.GreaterOrEqualToTheLeft(Repeating, NoPositionToTheLeft);

        Assert.Equal([-1, 0, 0, 2, 2], boundaries);
    }

    [Fact]
    public void SmallerToTheRight_RepeatingValues_StopsAtTheNearestStrictlySmallerPosition()
    {
        var boundaries = NearestBoundary.SmallerToTheRight(Repeating, PastTheLastPosition);

        Assert.Equal([1, 5, 3, 5, 5], boundaries);
    }

    [Fact]
    public void SmallerOrEqualToTheRight_RepeatingValues_StopsAtAnEqualNeighbour()
    {
        var boundaries = NearestBoundary.SmallerOrEqualToTheRight(Repeating, PastTheLastPosition);

        Assert.Equal([1, 3, 3, 5, 5], boundaries);
    }

    [Fact]
    public void GreaterToTheRight_RepeatingValues_ReportsNoneWhereAnEqualValueBlocksTheWay()
    {
        var boundaries = NearestBoundary.GreaterToTheRight(Repeating, PastTheLastPosition);

        Assert.Equal([5, 2, 5, 4, 5], boundaries);
    }

    [Fact]
    public void GreaterOrEqualToTheRight_RepeatingValues_StopsAtAnEqualNeighbour()
    {
        var boundaries = NearestBoundary.GreaterOrEqualToTheRight(Repeating, PastTheLastPosition);

        Assert.Equal([2, 2, 4, 4, 5], boundaries);
    }

    // The sentinel is the caller's, not the sweep's: a caller that needs to tell "no boundary"
    // from "boundary at position 0" cannot have that decided for it, so whatever it passes
    // comes back untouched in every slot the relation never satisfied.
    [Fact]
    public void SmallerToTheLeft_NoQualifyingPosition_WritesTheCallersOwnSentinel()
    {
        var boundaries = NearestBoundary.SmallerToTheLeft(Repeating, SomeOtherSentinel);

        Assert.Equal(
            [SomeOtherSentinel, SomeOtherSentinel, 1, SomeOtherSentinel, 3], boundaries);
    }

    [Fact]
    public void Sweep_FewerThanTwoPositions_HasNothingToCompareAgainst()
    {
        Assert.Empty(NearestBoundary.SmallerToTheLeft([], NoPositionToTheLeft));
        Assert.Equal([NoPositionToTheLeft], NearestBoundary.SmallerToTheLeft([4], NoPositionToTheLeft));
    }

    // The pinned expectations above are single cases on one array. This holds the sweep to the
    // definition instead - walk outward until the relation first holds - over arrays whose ties
    // run long enough that a rescan and a pop-based sweep have room to disagree. Only this
    // relation is checked this way: the other seven differ in nothing but the comparison the
    // sweep names, and their tie direction is what the expectations above pin.
    [Fact]
    public void SmallerToTheLeft_AgreesWithWalkingOutwardFromEveryPosition()
    {
        int[][] cases = [[4, 2, 4, 2, 4], [5, 5, 5, 5], [1, 2, 3, 4, 5], [5, 4, 3, 2, 1], [3, 1, 3, 1, 2, 1, 3]];

        foreach (var values in cases)
        {
            var walked = WalkOutward(values);
            var boundaries = NearestBoundary.SmallerToTheLeft(values, NoPositionToTheLeft);

            Assert.Equal(walked, boundaries);
        }
    }

    // The scan the sweep replaces, written the obvious way: from each position, step outward
    // until the relation first holds or the array runs out. Deliberately naive - give it a
    // stack and it stops being an independent check of a stack.
    private static int[] WalkOutward(ReadOnlySpan<int> values)
    {
        var walked = new int[values.Length];

        for (var position = 0; position < values.Length; position++)
        {
            walked[position] = NoPositionToTheLeft;
            for (var candidate = position - 1; candidate >= 0; candidate--)
            {
                if (values[candidate] < values[position])
                {
                    walked[position] = candidate;
                    break;
                }
            }
        }

        return walked;
    }
}
