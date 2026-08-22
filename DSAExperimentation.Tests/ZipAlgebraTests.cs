using DSAExperimentation.Trees;

namespace DSAExperimentation.Tests;

public readonly struct HeightDepthFirstAlgebra : IDepthFirstFoldAlgebra<TestNode, int>
{
    public static int Empty => 0;

    public static int Combine(TestNode node, IReadOnlyList<int> children)
    {
        var max = 0;

        foreach (var child in children)
        {
            if (child > max)
            {
                max = child;
            }
        }

        return 1 + max;
    }
}

public readonly struct MaxDepthBreadthFirstAlgebra : IBreadthFirstFoldAlgebra<TestNode, int>
{
    public static int Seed => 0;

    public static int Accumulate(int state, TestNode node, int depth)
        => depth > state ? depth : state;
}

public class ZipAlgebraTests
{
    private struct DfsZipCollectingMarker;
    private struct DfsZipMixedMarker;
    private struct DfsZipBothUnitFirstMarker;
    private struct DfsZipBothUnitSecondMarker;

    [Fact]
    public void BreadthFirstZip_CombinesBothAlgebrasInOnePass()
    {
        var root = TestTrees.NArySample();

        var result = BreadthFirstFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            ZipBreadthFirstAlgebra<
                TestNode,
                int,
                int,
                CountNodesBreadthFirstAlgebra,
                MaxDepthBreadthFirstAlgebra>,
            FoldResultPair<int, int>>(root);

        Assert.Equal(7, result.First);
        Assert.Equal(2, result.Second);
    }

    [Fact]
    public void DepthFirstZip_BothSidesCollectChildren_CombinesBothAlgebrasInOnePass()
    {
        var root = TestTrees.NArySample();

        var result = DepthFirstFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            ZipDepthFirstAlgebra<
                TestNode,
                int,
                int,
                CountNodesDepthFirstAlgebra,
                HeightDepthFirstAlgebra>,
            FoldResultPair<int, int>>(root);

        Assert.Equal(7, result.First);
        Assert.Equal(3, result.Second);
    }

    [Fact]
    public void DepthFirstZip_MixedCollectsChildResults_BothSidesStillCorrect()
    {
        var root = TestTrees.NArySample();

        var result = DepthFirstFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            ZipDepthFirstAlgebra<
                TestNode,
                int,
                Unit,
                CountNodesDepthFirstAlgebra,
                NodeVisitHooks<TestNode, RecordingNodeAction<DfsZipMixedMarker>>>,
            FoldResultPair<int, Unit>>(root);

        Assert.Equal(7, result.First);
        Assert.Equal(
            new[] { "A", "B", "E", "F", "C", "D", "G" },
            RecordingNodeAction<DfsZipMixedMarker>.Visited);
    }

    [Fact]
    public void DepthFirstZip_NeitherSideCollectsChildren_BothActionsFireOncePerNode()
    {
        var root = TestTrees.NArySample();

        DepthFirstFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            ZipDepthFirstAlgebra<
                TestNode,
                Unit,
                Unit,
                NodeVisitHooks<TestNode, RecordingNodeAction<DfsZipBothUnitFirstMarker>>,
                NodeVisitHooks<TestNode, RecordingNodeAction<DfsZipBothUnitSecondMarker>>>,
            FoldResultPair<Unit, Unit>>(root);

        Assert.Equal(
            new[] { "A", "B", "E", "F", "C", "D", "G" },
            RecordingNodeAction<DfsZipBothUnitFirstMarker>.Visited);
        Assert.Equal(
            RecordingNodeAction<DfsZipBothUnitFirstMarker>.Visited,
            RecordingNodeAction<DfsZipBothUnitSecondMarker>.Visited);
    }
}
