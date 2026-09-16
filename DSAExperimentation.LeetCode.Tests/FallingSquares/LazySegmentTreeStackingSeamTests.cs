using DSAExperimentation.LeetCode.FallingSquares;

namespace DSAExperimentation.LeetCode.Tests.FallingSquares;

// The seam between FallingSquaresSolution's two stackers.
// HeightsByBruteForceOverlapScan keeps a height per square and scans every earlier
// square for footprint overlap. HeightsByLazySegmentTree coordinate-compresses every
// footprint onto a dense leaf range and composes DataStructures'
// LazySegmentTree<int, int?, RangeAssignMaxOperation<int>> over it: one range-max
// Query asks what is already stacked under the square, one UpdateRange plants the
// new height across the same footprint.
//
// Coordinate compression is the contract between the two problems the lazily
// assigned tree has to answer at once. A square's right edge is stored as the leaf
// range [indexOf(left), indexOf(right) - 1], so two squares that only abut must land
// on disjoint leaves and two that overlap by one coordinate must land on a shared
// leaf - and RangeAssignMaxOperation's own rule (a newer assignment always wins) has
// to leave the taller of two stacks standing when a shorter square lands on it.
public sealed partial class LazySegmentTreeStackingSeamTests
{
    [Fact]
    public void Heights_LeetCodeExample_MatchesOverlapScan()
    {
        int[][] positions = [[1, 2], [2, 3], [6, 1]];

        Assert.Equal([2, 5, 5], FallingSquaresSolution.HeightsByLazySegmentTree(positions));
        AssertSameHeights(positions);
    }

    // Squares that only abut in half-open terms: the second must sit on the ground,
    // not on the first, so the compressed right edge has to be exclusive.
    [Fact]
    public void Heights_FootprintsThatOnlyTouch_MatchesOverlapScan()
    {
        int[][] positions = [[1, 2], [3, 2], [5, 1]];

        Assert.Equal([2, 2, 2], FallingSquaresSolution.HeightsByLazySegmentTree(positions));
        AssertSameHeights(positions);
    }

    // A narrower square landing on a wide one moves only part of the footprint, and
    // the wide square's own recorded height must survive under the parts of it the
    // narrower square never covered.
    [Fact]
    public void Heights_NarrowSquareOnAWideOne_MatchesOverlapScan()
    {
        int[][] positions = [[1, 10], [3, 2], [8, 1], [11, 4]];

        AssertSameHeights(positions);
    }

    // A tall square followed by a short one covering the whole of it: the second
    // square's own height is smaller than what is under it, so the running max has
    // to come from the height the square was planted at, not from its size.
    [Fact]
    public void Heights_ShortSquareCoveringATallOne_MatchesOverlapScan()
    {
        int[][] positions = [[1, 3], [1, 10], [1, 4]];

        AssertSameHeights(positions);
    }

    // The same footprint repeated: every square lands on top of the previous one, so
    // the range assignment has to overwrite the leaf rather than combine with it.
    [Fact]
    public void Heights_RepeatedIdenticalFootprint_MatchesOverlapScan()
    {
        int[][] positions = [[2, 4], [2, 4], [2, 4], [2, 4]];

        Assert.Equal([4, 8, 12, 16], FallingSquaresSolution.HeightsByLazySegmentTree(positions));
        AssertSameHeights(positions);
    }

    [Fact]
    public void Heights_SingleSquare_MatchesOverlapScan()
        => AssertSameHeights([[7, 5]]);

    // Footprints far apart in absolute coordinates but adjacent after compression:
    // the tree's leaves index the compressed edges, so a gap of a thousand units and
    // a gap of one are the same leaf step.
    [Fact]
    public void Heights_WidelySeparatedFootprints_MatchesOverlapScan()
    {
        int[][] positions = [[1, 2], [500, 3], [1000, 1], [500, 3]];

        AssertSameHeights(positions);
    }

    private static void AssertSameHeights(int[][] positions)
        => Assert.Equal(
            FallingSquaresSolution.HeightsByBruteForceOverlapScan(positions),
            FallingSquaresSolution.HeightsByLazySegmentTree(positions));
}
