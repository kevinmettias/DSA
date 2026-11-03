using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteNodeInALinkedListBenchmarks (ARCHITECTURE 17.9): the class has a single
// arm, so the generic arm check has no second strategy to reconcile it against, and the assertion
// comes from the arm's own documented contract instead: [GlobalSetup] hoists the run 0..Length-1 as the
// workload values, the arm rebuilds that run, targets the node at Length/2 and hands it to
// DeleteByNextValueCopy, which copies the successor's value into the target and splices the successor
// out, so the list the arm returns holds the run with exactly that one value gone.
public sealed partial class DeleteNodeInALinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    // The arm targets the node at the middle of the run: it rebuilds 0..Length-1, whose values equal
    // their own positions, so the node it splices out is the one holding Length over this.
    private const int MiddlePositionDivisor = 2;

    [Fact]
    public void NextValueCopy_RunOfTwoHundredValues_LeavesTheRunWithoutItsMiddleValue() =>
        Assert.Equal(
            Enumerable.Range(0, SmallestLength).Where(value => value != SmallestLength / MiddlePositionDivisor),
            LeetCodeWireFormat.FromLinkedList(BuildHarness().NextValueCopy()));

    private static DeleteNodeInALinkedListBenchmarks BuildHarness()
    {
        var harness = new DeleteNodeInALinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
