using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

public sealed partial class CheckedFoldTests
{
    [Fact]
    public void TryFold_TrueCycle_ReturnsFalseInsteadOfThrowing()
    {
        var root = TestGraphs.CycleWithLeaf();

        var succeeded = CheckedFold.TryFold<
            TestNode, TestTopology, ListChildren<TestNode>,
            CountNodesFoldAlgebra, int>(root, out _);

        Assert.False(succeeded);
    }

    [Fact]
    public void TryFold_NullRoot_SucceedsWithEmpty()
    {
        var succeeded = TryFold(null, [], out var spelled);

        Assert.True(succeeded);
        Assert.Equal("", spelled);
    }

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void TryFold_Tree_CombinesEachNodeWithItsChildrenInOrder()
    {
        var succeeded = TryFold(TestTrees.NArySample(), [], out var spelled);

        Assert.True(succeeded);
        Assert.Equal("ABEFCDG", spelled);
    }

    [Fact]
    public void TryFold_SharedDescendant_IsNotMistakenForACycle()
    {
        // D is reached a second time through C only after B's visit has finished with
        // it, so it is memoized rather than in progress: no cycle, and its result appears
        // under both parents.
        var succeeded = TryFold(TestGraphs.Diamond(), [], out var spelled);

        Assert.True(succeeded);
        Assert.Equal("ABDCD", spelled);
    }

    // D is reached through B and again through C, but the second arrival reads the memo: D is
    // combined once, before the B that needs it, and never again under C.
    [Fact]
    public void TryFold_SharedDescendant_IsCombinedOnce()
    {
        var combined = new List<string>();

        var succeeded = TryFold(TestGraphs.Diamond(), combined, out _);

        Assert.True(succeeded);
        Assert.Equal(["D", "B", "C", "A"], combined);
    }

    // A -> B -> C -> B: the root is not on the cycle, so the failure has to travel up
    // through A's own visit to reach TryFold.
    [Fact]
    public void TryFold_CycleBelowTheRoot_ReturnsFalse()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.Add(b);
        b.Children.Add(c);
        c.Children.Add(b);

        Assert.False(TryFold(a, [], out _));
    }

    [Fact]
    public void TryFold_SelfLoop_ReturnsFalse()
    {
        var a = new TestNode("A");
        a.Children.Add(a);

        Assert.False(TryFold(a, [], out _));
    }

    // A's children are B, which leads round the cycle back to A, then the leaf D. Once B's
    // branch finds the cycle the walk unwinds: D is never visited, and nothing on the cyclic
    // path is combined - a leaf D would have been the first node combined had it been reached.
    [Fact]
    public void TryFold_CycleUnderAnEarlierChild_NeverVisitsALaterSibling()
    {
        var combined = new List<string>();

        TryFold(TestGraphs.CycleWithLeaf(), combined, out _);

        Assert.Empty(combined);
    }

    // A -> [L, B], B -> C -> B: the leaf L is folded before the cycle under B is found, and the
    // walk stops there - A itself is never combined.
    [Fact]
    public void TryFold_CycleUnderALaterChild_StopsAfterTheEarlierChild()
    {
        var combined = new List<string>();

        var a = new TestNode("A");
        var leaf = new TestNode("L");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.Add(leaf);
        a.Children.Add(b);
        b.Children.Add(c);
        c.Children.Add(b);

        var succeeded = TryFold(a, combined, out _);

        Assert.False(succeeded);
        Assert.Equal(["L"], combined);
    }

    private static bool TryFold(TestNode? root, List<string> combined, out string spelled)
        => CheckedFold.TryFold<
            TestNode, TestTopology, ListChildren<TestNode>,
            RecordingNamesFoldAlgebra, string>(root, new RecordingNamesFoldAlgebra(combined), out spelled);
}
