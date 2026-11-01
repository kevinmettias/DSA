using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubtreeOfAnotherTreeBenchmarks (ARCHITECTURE 17.9): both arms answer with
// a bare bool, and the workload makes that bool decisive rather than merely agreed: [GlobalSetup]
// builds root as a left chain whose every node carries 1 and the half-sized subRoot as the same
// chain whose deepest node carries -1, a value root never holds, so no node of root can be
// structurally equal to subRoot and the honest verdict is false. The baseline is pinned to it; the
// generic arm check holds the other arm to the baseline's answer.
public sealed partial class SubtreeOfAnotherTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void IsSubtreeByRecursiveCompareAtEveryNode_SubRootWithAnAbsentValue_FindsNoSubtree() =>
        Assert.False(BuildHarness().IsSubtreeByRecursiveCompareAtEveryNode());

    private static SubtreeOfAnotherTreeBenchmarks BuildHarness()
    {
        var harness = new SubtreeOfAnotherTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
