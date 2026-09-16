using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteNodesFromLinkedListPresentInArrayBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - scanning the deleted-value array per node against this
// repo's Set<int> membership filter - so a harness whose arms disagree is timing two different problems.
// Setup draws both the list values and the deleted values from one fixed seed, and either strategy
// splices the chain it is handed, so each arm is given a list rebuilt from the same values. The reading
// is that surviving chain, whose documented shape is at most one node per value the list started with;
// the same Length must rebuild the same lists and with them the same survivors.
public sealed partial class DeleteNodesFromLinkedListPresentInArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every value the list started with is either deleted or kept, so nothing can be added.
    private const int MinimumSurvivorCount = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.InRange(
            Values(BuildHarness().ArrayScan()).Count,
            MinimumSurvivorCount,
            SmallestLength);
        Assert.Equal(
            AnswerText.Of(Values(BuildHarness().ArrayScan())),
            AnswerText.Of(Values(BuildHarness().ArrayScan())));
    }

    [Fact]
    public void ArrayScan_TwoHundredSeededValues_AgreesWithSetFilter()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Values(harness.SetFilter())),
            AnswerText.Of(Values(harness.ArrayScan())));
    }

    [Fact]
    public void SetFilter_TwoHundredSeededValues_AgreesWithArrayScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Values(harness.ArrayScan())),
            AnswerText.Of(Values(harness.SetFilter())));
    }

    // Both arms report the surviving chain as that internal node type through an object, so the test
    // reads the values off it rather than comparing nodes.
    private static List<int> Values(object? head)
    {
        var values = new List<int>();

        for (var node = head as SinglyLinkedListNode<int>; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static DeleteNodesFromLinkedListPresentInArrayBenchmarks BuildHarness()
    {
        var harness = new DeleteNodesFromLinkedListPresentInArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
