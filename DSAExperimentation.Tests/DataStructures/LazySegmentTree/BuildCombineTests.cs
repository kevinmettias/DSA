using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

// Companion of BuildCombine, the adapter SegmentTreeBuild.Fill is handed so the shared
// build recursion can serve this tree's IRangeUpdateOperation: its whole surface is the
// operation's own Combine and Identity, forwarded rather than re-derived. Pinned here
// against the operation itself - never against a hand-computed literal - because that
// forwarding IS the claim, and pinned through the tree because the adapter is private to
// it and the tree's internal nodes are where its two members are observable.
public sealed partial class BuildCombineTests
{
    // Combine forwards whatever operation the tree was built with, so the same shared
    // recursion serves a max tree as well as a sum tree: a root that came from any one
    // fixed combine would answer one of these two with the other's arithmetic.
    [Fact]
    public void Combine_ForwardsWhateverOperationTheTreeWasBuiltWith()
    {
        int[] pair = [3, 4];
        var maxTree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(pair);
        var sumTree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(pair);

        Assert.Equal(
            RangeAssignMaxOperation<int>.Combine(pair[0], pair[1]),
            maxTree.Query(0, pair.Length - 1));
        Assert.Equal(
            RangeAddSumOperation<int>.Combine(pair[0], pair[1]),
            sumTree.Query(0, pair.Length - 1));
    }

    // A query answers each branch disjoint from it with TOperation.Identity, and
    // RangeAssignMaxOperation<int>'s identity is int.MinValue: a query confined to one
    // element leaves a disjoint sibling at every level, so a default(int) standing in for
    // the identity would win the combine and this would read 0 instead of the element.
    [Fact]
    public void Identity_DisjointQueryBranch_IsTheOperationsOwnIdentityRatherThanTheElementDefault()
    {
        int[] values = [-5, -8, -3];
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(values);

        for (var index = 0; index < values.Length; index++)
        {
            Assert.Equal(values[index], tree.Query(index, index));
        }
    }
}
