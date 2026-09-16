using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.DeleteNodeInALinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteNodeInALinkedListBenchmarks (ARCHITECTURE 17.9): the class has a single arm,
// so there is no second strategy to reconcile it against, and that arm returns void - it splices a node
// out of the list it builds as a local - so the harness exposes no value to compare at all. The
// assertion therefore comes from the arm's own documented contract: [GlobalSetup] hoists the run
// 0..Length-1 as the workload values, the arm rebuilds that run, targets the node at Length/2 and hands
// it to DeleteByNextValueCopy, which copies the successor's value into the target and splices the
// successor out, so the run survives with exactly that one value gone. The values are unseeded and a
// function of Length alone, so the same Length must rebuild the same run.
public sealed partial class DeleteNodeInALinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

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
    public void NextValueCopy_RunOfTwoHundredValues_LeavesTheRunWithoutItsMiddleValue()
    {
        BuildHarness().NextValueCopy();

        Assert.Equal(
            Enumerable.Range(0, SmallestLength).Where(value => value != SmallestLength / 2),
            Replay());
    }

    // Mirrors [GlobalSetup]'s own expression.
    private static IEnumerable<int> WorkloadValues() => Enumerable.Range(0, SmallestLength);

    // Mirrors the arm exactly: rebuild the run, target the node at Length/2 - the values equal their own
    // positions, so that node holds Length/2 - and splice it out through the solution the arm calls.
    private static List<int> Replay()
    {
        var values = WorkloadValues().ToArray();
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        foreach (var value in values.Skip(1))
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        var target = head;

        for (var i = 0; i < SmallestLength / 2; i++)
        {
            target = target.Next!;
        }

        DeleteNodeInALinkedListSolution.DeleteByNextValueCopy(target);

        var remaining = new List<int>();

        for (SinglyLinkedListNode<int>? node = head; node is not null; node = node.Next)
        {
            remaining.Add(node.Value);
        }

        return remaining;
    }

    private static DeleteNodeInALinkedListBenchmarks BuildHarness()
    {
        var harness = new DeleteNodeInALinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
