using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseLinkedListBenchmarks (ARCHITECTURE 17.9): the class has a single arm,
// so there is no second strategy to reconcile it against and the assertion has to be derived rather
// than agreed. [GlobalSetup] builds the values 1..Length in order, which fixes the reversal
// independently of the arm: the answer is that same sequence rendered backwards, so the oracle below
// is stated from the fixture's shape and not read back out of the arm.
//
// The arm returns the new head as object? (the node type is internal, CS0050) and AnswerText cannot
// render a linked node - it is not an enumerable sequence - so each result is walked into its values
// first. Both the arm and the Setup call rebuild the chain from the hoisted values inside the
// measured call (the rewire mutates the nodes it is handed), so one harness is safe to call twice in
// either order.
public sealed partial class ReverseLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().IterativeRewire())),
            AnswerText.Of(ValuesOf(BuildHarness().IterativeRewire())));

    [Fact]
    public void IterativeRewire_AscendingValues_ProducesTheDescendingSequence() =>
        Assert.Equal(ExpectedDescending(SmallestLength), ValuesOf(BuildHarness().IterativeRewire()));

    private static int[] ValuesOf(object? head)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // [GlobalSetup] fills the chain with 1..Length in ascending order, so reversing it is that same
    // range rendered backwards.
    private static int[] ExpectedDescending(int length) => [.. Enumerable.Range(1, length).Reverse()];

    private static ReverseLinkedListBenchmarks BuildHarness()
    {
        var harness = new ReverseLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
