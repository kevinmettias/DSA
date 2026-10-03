using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinaryTreeMaximumPathSumBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm, so there is no second strategy to reconcile it against and the assertion has to come
// from the workload's documented construction instead - a gapless level-order array of values drawn
// from seeded Random(124) across [-1000, 1000], turned into its complete tree. The expected sum is
// derived here from that rebuilt array by an index-based pass that never builds a tree at all, so it
// is not a restatement of the arm. The same NodeCount must always rebuild the same tree, and so the
// same sum.
public sealed partial class BinaryTreeMaximumPathSumBenchmarksTests
{
    private const int SmallestNodeCount = 300;
    private const int RandomSeed = 124;
    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;

    // In a gapless level order, node i's children sit at ChildrenPerNode * i + 1 and the slot after it.
    private const int ChildrenPerNode = 2;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().GainRecursion(), BuildHarness().GainRecursion());

    [Fact]
    public void GainRecursion_SeededCompleteTree_ReturnsTheIndependentlyComputedSum() =>
        Assert.Equal(IndependentMaxPathSum(RebuildLevelOrder()), BuildHarness().GainRecursion());

    private static BinaryTreeMaximumPathSumBenchmarks BuildHarness()
    {
        var harness = new BinaryTreeMaximumPathSumBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // The benchmark's own draw, rebuilt from its documented shape.
    private static int[] RebuildLevelOrder() =>
        SeededDraws.Values(SmallestNodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));

    // Walking the indices from the last to the first visits every child before its parent, so each
    // node's best downward gain is ready by the time its parent reads it. A path peaks at exactly one
    // node, taking that node plus whichever child gains are positive.
    private static int IndependentMaxPathSum(int[] levelOrder)
    {
        var gain = new int[levelOrder.Length];
        var best = int.MinValue;

        for (var node = levelOrder.Length - 1; node >= 0; node--)
        {
            var left = PositiveGain(gain, (ChildrenPerNode * node) + 1);
            var right = PositiveGain(gain, (ChildrenPerNode * node) + ChildrenPerNode);
            best = Math.Max(best, levelOrder[node] + left + right);
            gain[node] = levelOrder[node] + Math.Max(left, right);
        }

        return best;
    }

    private static int PositiveGain(int[] gain, int child) => child < gain.Length ? Math.Max(0, gain[child]) : 0;
}
