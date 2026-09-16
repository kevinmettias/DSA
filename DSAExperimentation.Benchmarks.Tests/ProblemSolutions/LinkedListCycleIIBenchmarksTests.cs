using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListCycleIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the visited-set baseline against this repo's own Floyd detector -
// so a harness whose arms disagree is timing two different problems. Both return the cycle's entry
// node as object? (the node type is internal, CS0050), and AnswerText renders every node as its type
// name, so rendering the two answers would compare equal whatever either arm returned; the arms are
// compared by reference identity instead, which is what "the same node starts the cycle" means.
//
// Setup builds a Length-node chain whose tail rejoins its head, so the entry is the head node - the
// one carrying HeadValue. Two independently built harnesses hold different node instances, so the
// Setup test compares that value rather than the instances.
public sealed partial class LinkedListCycleIIBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int HeadValue = 0;

    [Fact]
    public void Setup_SameLength_RebuildsAChainWithTheSameCycleEntry() =>
        Assert.Equal(EntryValue(BuildHarness().VisitedSet()), EntryValue(BuildHarness().VisitedSet()));

    [Fact]
    public void VisitedSet_TailRejoinsHead_FindsTheSameEntryAsFloydCycleDetection()
    {
        var harness = BuildHarness();

        Assert.Equal(HeadValue, EntryValue(harness.VisitedSet()));
        Assert.Same(harness.VisitedSet(), harness.FloydCycleDetection());
    }

    [Fact]
    public void FloydCycleDetection_TailRejoinsHead_FindsTheSameEntryAsVisitedSet()
    {
        var harness = BuildHarness();

        Assert.Equal(HeadValue, EntryValue(harness.FloydCycleDetection()));
        Assert.Same(harness.VisitedSet(), harness.FloydCycleDetection());
    }

    // The arm's result is the internal node type boxed back into object? (CS0050), which is why the
    // entry is read back out of the box rather than named by the arm's own signature.
    private static int EntryValue(object? entry) => ((SinglyLinkedListNode<int>)entry!).Value;

    private static LinkedListCycleIIBenchmarks BuildHarness()
    {
        var harness = new LinkedListCycleIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
