using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding.Dags;

// TestTopology promises ITreeTopology, which extends IDagTopology, so it is usable
// wherever DagFold asks for a DAG - and nothing stops it being pointed at a diamond.
public sealed partial class DagFoldTests
{
    private struct NullRootMarker;
    private struct TreeMarker;
    private struct DiamondMarker;

    [Fact]
    public void Fold_NullRoot_ReturnsEmpty()
        => Assert.Equal(RecordingNamesFoldAlgebra<NullRootMarker>.Empty, Fold<NullRootMarker>(null));

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Fold_Tree_CombinesEachNodeWithItsChildrenInOrder()
    {
        var spelled = Fold<TreeMarker>(TestTrees.NArySample());

        Assert.Equal("ABEFCDG", spelled);
        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("E", 2), ("F", 2), ("C", 1), ("D", 1), ("G", 2) },
            RecordingNamesFoldAlgebra<TreeMarker>.Entered);
    }

    [Fact]
    public void Fold_SharedDescendant_EntersItOnceAndReusesItsResult()
    {
        // D's result appears under both parents, but the second arrival reads the memo:
        // D is entered once, at the depth of the path that reached it first.
        var spelled = Fold<DiamondMarker>(TestGraphs.Diamond());

        Assert.Equal("ABDCD", spelled);
        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("D", 2), ("C", 1) },
            RecordingNamesFoldAlgebra<DiamondMarker>.Entered);
    }

    private static string Fold<TMarker>(TestNode? root)
        where TMarker : struct
        => DagFold.Fold<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingNamesFoldAlgebra<TMarker>, string>(root);
}
