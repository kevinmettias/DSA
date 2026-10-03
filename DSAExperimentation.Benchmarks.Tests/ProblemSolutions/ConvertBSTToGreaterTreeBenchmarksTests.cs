using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConvertBSTToGreaterTreeBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin, since it calls every arm on a harness nothing else has touched: the class's documented shape that each arm
// clones the shared tree before transforming it. A second call on the same harness must therefore transform the
// same untransformed values again, rather than re-transforming an already-transformed tree. Each arm returns the
// transformed root as object? (the node type is internal, CS0050), so the two trees are compared by
// AnswerGraphText's field walk.
public sealed partial class ConvertBSTToGreaterTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void ManualReverseInOrder_CalledTwiceOnOneHarness_TransformsTheSameUntransformedTree()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(harness.ManualReverseInOrder()), AnswerGraphText.Of(harness.ManualReverseInOrder()));
    }

    [Fact]
    public void InOrderTraversalHooks_CalledTwiceOnOneHarness_TransformsTheSameUntransformedTree()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(harness.InOrderTraversalHooks()), AnswerGraphText.Of(harness.InOrderTraversalHooks()));
    }

    private static ConvertBSTToGreaterTreeBenchmarks BuildHarness()
    {
        var harness = new ConvertBSTToGreaterTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
