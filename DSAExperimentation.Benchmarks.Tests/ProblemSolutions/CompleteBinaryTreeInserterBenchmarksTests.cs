using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CompleteBinaryTreeInserterBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - re-walking the whole tree per Insert against the
// pre-seeded incomplete-node queue - so a harness whose arms disagree is timing two different
// problems. Setup holds only the insert values; the perfect tree is rebuilt inside every arm call,
// because a run mutates it, so one harness is safe to call twice in either order.
//
// The workload shape the reading depends on is therefore asserted through the arm's own answer, and
// that answer is a proxy: LC 919's tree stays complete, so inserts fill open slots in level order and
// the last of the sequence lands on the final slot of the SmallestNodeCount + InsertCount node
// complete tree, whose parent in heap layout is one less than the slot over BranchingFactor. Both
// arms must land there, and the same arm must land there again on a second, independently built
// harness. Agreement here witnesses that both arms perform the whole insertion sequence and agree on
// where it ended - not what every intermediate parent was.
public sealed partial class CompleteBinaryTreeInserterBenchmarksTests
{
    private const int SmallestNodeCount = 63;
    private const int InsertCount = 200;

    [Fact]
    public void Setup_PerfectTreePlusTwoHundredInserts_EndsTheSequenceOnTheFinalOpenParent()
    {
        var lastSlot = SmallestNodeCount + InsertCount - 1;
        var finalOpenParent = (lastSlot - 1) / AlgorithmConstants.BranchingFactor;

        Assert.Equal(finalOpenParent, BuildHarness().NaiveRescanPerInsert());
        Assert.Equal(BuildHarness().NaiveRescanPerInsert(), BuildHarness().NaiveRescanPerInsert());
    }

    [Fact]
    public void NaiveRescanPerInsert_TwoHundredInsertsOnAPerfectTree_AgreesWithQueueTrackedInserter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QueueTrackedInserter(), harness.NaiveRescanPerInsert());
    }

    [Fact]
    public void QueueTrackedInserter_TwoHundredInsertsOnAPerfectTree_AgreesWithNaiveRescanPerInsert()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRescanPerInsert(), harness.QueueTrackedInserter());
    }

    private static CompleteBinaryTreeInserterBenchmarks BuildHarness()
    {
        var harness = new CompleteBinaryTreeInserterBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
