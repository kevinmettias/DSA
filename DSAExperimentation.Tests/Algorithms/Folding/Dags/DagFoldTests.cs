using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding.Dags;

// TestTopology promises ITreeTopology, which extends IDagTopology, so it is usable
// wherever DagFold asks for a DAG - and nothing stops it being pointed at a diamond.
public sealed partial class DagFoldTests
{
    [Fact]
    public void Fold_NullRoot_ReturnsEmpty()
        => Assert.Equal("", Fold(null, []));

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Fold_Tree_CombinesEachNodeWithItsChildrenInOrder()
    {
        var combined = new List<string>();

        var spelled = Fold(TestTrees.NArySample(), combined);

        Assert.Equal("ABEFCDG", spelled);
        Assert.Equal(["E", "F", "B", "C", "G", "D", "A"], combined);
    }

    [Fact]
    public void Fold_SharedDescendant_CombinesItOnceAndReusesItsResult()
    {
        var combined = new List<string>();

        // D's result appears under both parents, but the second arrival reads the memo:
        // D is combined once, for the path that reached it first.
        var spelled = Fold(TestGraphs.Diamond(), combined);

        Assert.Equal("ABDCD", spelled);
        Assert.Equal(["D", "B", "C", "A"], combined);
    }

    private static string Fold(TestNode? root, List<string> combined)
        => DagFold.Fold<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingNamesFoldAlgebra, string>(root, new RecordingNamesFoldAlgebra(combined));
}
