using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for SkewedTreeFoldBenchmarks (ARCHITECTURE 17.9): same fold and algebra as the
// balanced case, over the degenerate right-only chain where recursion depth grows one step per
// node. The two arms are competing fold-evaluation strategies for one question, and SizeAlgebra
// answers that question with the node count.
public sealed partial class SkewedTreeFoldBenchmarksTests
{
    private const int SmallestNodeCount = 1_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().Recursive()),
            AnswerText.Of(BuildHarness().Recursive()));

    [Fact]
    public void Recursive_AgreesWithIterative()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Recursive(), harness.Iterative());
    }

    [Fact]
    public void Iterative_AgreesWithRecursive()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Iterative(), harness.Recursive());
    }

    [Fact]
    public void Recursive_SizeAlgebra_CountsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().Recursive());

    [Fact]
    public void Iterative_SizeAlgebra_CountsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().Iterative());

    private static SkewedTreeFoldBenchmarks BuildHarness()
    {
        var harness = new SkewedTreeFoldBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
