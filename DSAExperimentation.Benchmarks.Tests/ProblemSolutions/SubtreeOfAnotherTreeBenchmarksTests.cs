using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubtreeOfAnotherTreeBenchmarks (ARCHITECTURE 17.9): both arms are
// SubtreeOfAnotherTreeSolution's, so a harness whose arms disagree is timing two different questions.
// Both answer with a bare bool, and the workload makes that bool decisive rather than merely agreed:
// [GlobalSetup] builds root as a left chain whose every node carries 1 and the half-sized subRoot as
// the same chain whose deepest node carries -1, a value root never holds, so no node of root can be
// structurally equal to subRoot and the honest verdict both arms are pinned to is false.
//
// Both arms only read the trees they are handed, so one harness is safe to call twice in either
// order; the trees themselves are private, so the Setup test can only compare what two harnesses
// built through the arms.
public sealed partial class SubtreeOfAnotherTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            (BuildHarness().IsSubtreeByRecursiveCompareAtEveryNode(), BuildHarness().IsSubtreeBySerializeThenKmpSearch()),
            (BuildHarness().IsSubtreeByRecursiveCompareAtEveryNode(), BuildHarness().IsSubtreeBySerializeThenKmpSearch()));

    [Fact]
    public void IsSubtreeByRecursiveCompareAtEveryNode_SubRootWithAnAbsentValue_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsSubtreeBySerializeThenKmpSearch(), harness.IsSubtreeByRecursiveCompareAtEveryNode());
        Assert.False(harness.IsSubtreeByRecursiveCompareAtEveryNode());
    }

    [Fact]
    public void IsSubtreeBySerializeThenKmpSearch_SubRootWithAnAbsentValue_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsSubtreeByRecursiveCompareAtEveryNode(), harness.IsSubtreeBySerializeThenKmpSearch());
        Assert.False(harness.IsSubtreeBySerializeThenKmpSearch());
    }

    private static SubtreeOfAnotherTreeBenchmarks BuildHarness()
    {
        var harness = new SubtreeOfAnotherTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
