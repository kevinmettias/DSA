using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertionSortListBenchmarks (ARCHITECTURE 17.9): the class has a single arm
// - the dummy-headed insertion sort - so there is no second strategy to reconcile it against and
// the assertion has to come from the arm's own declared contract instead. It returns the sorted
// list's head as object (the node type is internal, so a public [Benchmark] method cannot name it
// as a return type), which is the real answer rather than a proxy; AnswerText cannot render it,
// because a linked node is not an enumerable sequence, so each result is walked into its values
// first. That sequence is decisive: [GlobalSetup] shuffles the values 1..Length with a fixed seed,
// so an insertion sort of them must return exactly 1..Length ascending - a value set the fixture
// fixes and the arm cannot influence.
public sealed partial class InsertionSortListBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameShuffledValues() =>
        Assert.Equal(RenderedValues(BuildHarness().DummyHeadInsertion()), RenderedValues(BuildHarness().DummyHeadInsertion()));

    [Fact]
    public void DummyHeadInsertion_ShuffledValues_SortsThemBackIntoAscendingOrder()
    {
        var harness = BuildHarness();

        Assert.Equal(AscendingValues(SmallestLength), ValuesOf(harness.DummyHeadInsertion()));
    }

    private static InsertionSortListBenchmarks BuildHarness()
    {
        var harness = new InsertionSortListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static string RenderedValues(object? answer) => AnswerText.Of(ValuesOf(answer));

    private static int[] ValuesOf(object? answer)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)answer; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // SeededSequences.ShuffledOneTo permutes 1..Length, so sorting it restores the range itself.
    private static int[] AscendingValues(int length) => [.. Enumerable.Range(1, length)];
}
