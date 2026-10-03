using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CompleteBinaryTreeInserterBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: every parent the insertion sequence reports, known from the workload's construction rather than from either
// arm. Both arms return the parent value of every Insert, in order. LC 919's tree stays complete, so inserts fill
// open slots in level order: the i-th insert onto the SmallestNodeCount-node perfect tree lands on slot
// SmallestNodeCount + i, whose parent in heap layout is one less than the slot over BranchingFactor - and
// Fixtures.BinaryTrees.Balanced gives every node its own index as its value.
public sealed partial class CompleteBinaryTreeInserterBenchmarksTests
{
    private const int SmallestNodeCount = 63;
    private const int InsertCount = 200;

    [Fact]
    public void NaiveRescanPerInsert_TwoHundredInsertsOnAPerfectTree_ReportsEachSlotsHeapParent() =>
        Assert.Equal(HeapParentsOfInsertedSlots(), BuildHarness().NaiveRescanPerInsert());

    [Fact]
    public void QueueTrackedInserter_TwoHundredInsertsOnAPerfectTree_ReportsEachSlotsHeapParent() =>
        Assert.Equal(HeapParentsOfInsertedSlots(), BuildHarness().QueueTrackedInserter());

    private static IEnumerable<int> HeapParentsOfInsertedSlots() =>
        Enumerable.Range(SmallestNodeCount, InsertCount).Select(slot => (slot - 1) / AlgorithmConstants.BranchingFactor);

    private static CompleteBinaryTreeInserterBenchmarks BuildHarness()
    {
        var harness = new CompleteBinaryTreeInserterBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
