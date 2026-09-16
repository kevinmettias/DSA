using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionListBenchmarks (ARCHITECTURE 17.9): its two arms are
// PartitionListSolution's, competing strategies for the same partition - collecting the values and
// rebuilding the list against threading the existing nodes onto two chains - so a harness whose
// arms disagree is timing two different problems. The splice arm mutates the list it is handed, but
// the benchmark builds that list inside each call rather than caching it, so one harness is safe to
// call in either order. Both arms are declared as returning object (the node type is internal,
// CS0050), so the tests read the value sequence off the returned list and compare that, which is
// the answer LC 86 actually asks for.
public sealed partial class PartitionListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().PointerSplice())),
            AnswerText.Of(ValuesOf(BuildHarness().PointerSplice())));

    [Fact]
    public void ArrayRebuild_DescendingValues_AgreesWithPointerSplice()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ValuesOf(harness.PointerSplice())), AnswerText.Of(ValuesOf(harness.ArrayRebuild())));
    }

    [Fact]
    public void PointerSplice_DescendingValues_AgreesWithArrayRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ValuesOf(harness.ArrayRebuild())), AnswerText.Of(ValuesOf(harness.PointerSplice())));
    }

    // Both arms hand back the internal node type through an object, so walking it into its value
    // sequence puts them into one comparable form without changing what either computed.
    private static int[] ValuesOf(object? answer)
    {
        var values = new List<int>();

        for (var node = answer as SinglyLinkedListNode<int>; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    private static PartitionListBenchmarks BuildHarness()
    {
        var harness = new PartitionListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
