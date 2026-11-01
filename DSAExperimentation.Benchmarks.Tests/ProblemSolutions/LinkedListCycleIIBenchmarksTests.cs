using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListCycleIIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on the rendering of their answers, so this asserts what a rendering
// cannot. Both arms return the cycle's entry node, and here they are compared by reference identity
// on one harness, which is what "the same node starts the cycle" means: a rendering compares the
// shape reachable from a node, and two different nodes of one cycle reach the same shape.
//
// Setup builds a Length-node chain whose tail rejoins its head, so the entry is the head node - the
// one carrying HeadValue, which is asserted on the baseline independently of either arm.
public sealed partial class LinkedListCycleIIBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int HeadValue = 0;

    [Fact]
    public void VisitedSet_TailRejoinsHead_FindsTheSameEntryAsFloydCycleDetection()
    {
        var harness = BuildHarness();

        Assert.Equal(HeadValue, EntryValue(harness.VisitedSet()));
        Assert.Same(harness.VisitedSet(), harness.FloydCycleDetection());
    }

    private static int EntryValue(SinglyLinkedListNode<int>? entry)
    {
        Assert.NotNull(entry);

        return entry.Value;
    }

    private static LinkedListCycleIIBenchmarks BuildHarness()
    {
        var harness = new LinkedListCycleIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
