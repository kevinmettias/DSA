using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for BalancedTreeFoldBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// fold-evaluation strategies for one question on one balanced tree, so a harness whose arms
// disagree is timing two different problems. SizeAlgebra answers with the node count, so the
// seeded fixture pins both arms to a decisive value as well as to each other.
public sealed partial class BalancedTreeFoldBenchmarksTests
{
    private const int SmallestNodeCount = 10_000;

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

    private static BalancedTreeFoldBenchmarks BuildHarness()
    {
        var harness = new BalancedTreeFoldBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
