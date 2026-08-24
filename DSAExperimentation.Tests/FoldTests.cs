using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public sealed class FoldTests
{
    [Fact]
    public void Fold_CountsNodes()
    {
        var root = TestTrees.NArySample();

        var count = TreeFold.Fold<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            CountNodesFoldAlgebra,
            int>(root);

        Assert.Equal(7, count);
    }

    [Fact]
    public void Fold_NullRoot_ReturnsEmpty()
    {
        var count = TreeFold.Fold<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            CountNodesFoldAlgebra,
            int>(null);

        Assert.Equal(0, count);
    }

    [Fact]
    public void Fold_IterativeEvaluation_MatchesRecursiveEvaluation()
    {
        // A pure algebra's result must not depend on evaluation strategy - only the
        // Combine calls' timing differs, not the value they build.
        var root = TestTrees.NArySample();
        var recursiveHeight = HeightViaEvaluation<RecursiveFoldEvaluation<TestNode>>(root);
        var iterativeHeight = HeightViaEvaluation<IterativeFoldEvaluation<TestNode>>(root);

        Assert.Equal(3, recursiveHeight);
        Assert.Equal(recursiveHeight, iterativeHeight);
    }

    private static int HeightViaEvaluation<TStrategy>(TestNode root)
        where TStrategy : struct, IFoldEvaluationStrategy<TestNode>
        => TreeFold.Fold<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            TStrategy,
            HeightAlgebra<TestNode>,
            int>(root);

    [Fact]
    public void Fold_IterativeEvaluation_NullRoot_ReturnsEmpty()
    {
        var count = TreeFold.Fold<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            IterativeFoldEvaluation<TestNode>,
            CountNodesFoldAlgebra,
            int>(null);

        Assert.Equal(0, count);
    }
}
