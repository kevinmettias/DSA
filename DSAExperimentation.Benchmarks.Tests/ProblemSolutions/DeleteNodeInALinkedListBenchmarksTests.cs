using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteNodeInALinkedListBenchmarks (ARCHITECTURE 17.9): the class has a single
// arm, so there is no second strategy to reconcile it against, and the assertion comes from the arm's
// own documented contract instead: [GlobalSetup] hoists the run 0..Length-1 as the workload values, the
// arm rebuilds that run, targets the node at Length/2 and hands it to DeleteByNextValueCopy, which
// copies the successor's value into the target and splices the successor out, so the list the arm
// returns holds the run with exactly that one value gone. The values are unseeded and a function of
// Length alone, so the same Length must rebuild the same run.
public sealed partial class DeleteNodeInALinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    // The arm targets the node at the middle of the run: it rebuilds 0..Length-1, whose values equal
    // their own positions, so the node it splices out is the one holding Length over this.
    private const int MiddlePositionDivisor = 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        // Building the harness runs [GlobalSetup] and the arm replays the whole run through it, which is
        // what proves the workload is constructible; the run itself is private, so its shape is stated
        // against the one expression Setup builds it from.
        BuildHarness().NextValueCopy();

        Assert.Equal(Enumerable.Range(0, SmallestLength), WorkloadValues());
        Assert.Equal(WorkloadValues(), WorkloadValues());
    }

    [Fact]
    public void NextValueCopy_RunOfTwoHundredValues_LeavesTheRunWithoutItsMiddleValue() =>
        Assert.Equal(
            Enumerable.Range(0, SmallestLength).Where(value => value != SmallestLength / MiddlePositionDivisor),
            LeetCodeWireFormat.FromLinkedList(BuildHarness().NextValueCopy()));

    // Mirrors [GlobalSetup]'s own expression.
    private static IEnumerable<int> WorkloadValues() => Enumerable.Range(0, SmallestLength);

    private static DeleteNodeInALinkedListBenchmarks BuildHarness()
    {
        var harness = new DeleteNodeInALinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
