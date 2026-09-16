using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OperationsOnTreeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same locked-descendant answers - a whole-tree scan per query against a
// subtree-only walk - so a harness whose arms disagree is timing two different problems. Setup builds
// one heap-shaped tree per arm from a fixed parent array and locks the same every-seventh node in
// each, so the same NodeCount must rebuild the same two trees and the same queries; both arms only
// read their tree, so one harness is safe to call twice in either order.
public sealed partial class OperationsOnTreeBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().WholeTreeScanEachQuery(), BuildHarness().WholeTreeScanEachQuery());

    [Fact]
    public void WholeTreeScanEachQuery_SmallestNodeCount_AgreesWithSubtreeDepthFirstSearchEachQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SubtreeDepthFirstSearchEachQuery(), harness.WholeTreeScanEachQuery());
    }

    [Fact]
    public void SubtreeDepthFirstSearchEachQuery_SmallestNodeCount_AgreesWithWholeTreeScanEachQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.WholeTreeScanEachQuery(), harness.SubtreeDepthFirstSearchEachQuery());
    }

    private static OperationsOnTreeBenchmarks BuildHarness()
    {
        var harness = new OperationsOnTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
