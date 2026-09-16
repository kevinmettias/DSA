using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

public sealed partial class ZipFoldAlgebraTests
{
    private static readonly TestNode Node = new("A");

    private static (int A, int B) Combine(IReadOnlyList<(int, int)> children) =>
        ZipFoldAlgebra<TestNode, int, int, SizeAlgebra<TestNode>, HeightAlgebra<TestNode>>.Combine(Node, children);

    [Fact]
    public void Empty_PairsTheTwoAlgebrasOwnEmptyValues() =>
        Assert.Equal(
            (SizeAlgebra<TestNode>.Empty, HeightAlgebra<TestNode>.Empty),
            ZipFoldAlgebra<TestNode, int, int, SizeAlgebra<TestNode>, HeightAlgebra<TestNode>>.Empty);

    [Fact]
    public void Combine_Leaf_RunsBothAlgebrasIndependently() => Assert.Equal((1, 1), Combine([]));

    [Fact]
    public void Combine_SplitsTheChildPairsBackIntoTwoIndependentLists()
    {
        // Size sums (1+2+3), height maxes (1+max(2,3)) - proving neither algebra
        // sees the other's results.
        Assert.Equal((6, 4), Combine([(2, 2), (3, 3)]));
    }

    [Fact]
    public void Combine_MatchesRunningEachAlgebraSeparately()
    {
        var children = new (int, int)[] { (4, 2), (2, 3) };

        var zipped = Combine(children);

        var expectedSize = SizeAlgebra<TestNode>.Combine(Node, [4, 2]);
        var expectedHeight = HeightAlgebra<TestNode>.Combine(Node, [2, 3]);

        Assert.Equal(expectedSize, zipped.A);
        Assert.Equal(expectedHeight, zipped.B);
    }

    [Fact]
    public void Enter_ForwardsToBothAlgebrasWithoutThrowing() =>
        Assert.Null(Record.Exception(
            () => ZipFoldAlgebra<TestNode, int, int, SizeAlgebra<TestNode>, HeightAlgebra<TestNode>>.Enter(Node, 0)));
}
