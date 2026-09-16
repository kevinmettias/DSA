using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeKSortedListsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - flattening every list and sorting the values against the
// K-way heap merge - so a harness whose arms disagree is timing two different problems. Both arms
// report the merged chain as an object over the data structure's internal node type, so the test reads
// the values off it rather than comparing nodes; the merge is order-defining, so the order-sensitive
// rendering is the right comparison.
//
// The workload is its own oracle: list `offset` holds offset, ListCount + offset, 2 * ListCount +
// offset, ... so ListCount lists of ValuesPerList values tile 0 .. ListCount * ValuesPerList - 1
// exactly once each, and the answer is documented to be that whole range in ascending order. Both arms
// splice the input nodes' own Next pointers, so each rebuilds its lists inside the measured call and
// one harness is safe to read twice in either order. Setup derives the values from the two counts
// alone, so the same ListCount must rebuild the same lists.
public sealed partial class MergeKSortedListsBenchmarksTests
{
    private const int SmallestListCount = 8;

    // Mirrors the harness's own per-list value count, which decides how far the tiled range runs.
    private const int ValuesPerList = 64;

    private const int ExpectedMergedValueCount = SmallestListCount * ValuesPerList;

    [Fact]
    public void Setup_SameListCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(Values(BuildHarness().FlattenSort())),
            AnswerText.Of(Values(BuildHarness().FlattenSort())));

    [Fact]
    public void FlattenSort_TiledValueLists_AgreesWithMergeByHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Enumerable.Range(0, ExpectedMergedValueCount)),
            AnswerText.Of(Values(harness.FlattenSort())));
        Assert.Equal(
            AnswerText.Of(Values(harness.MergeByHeap())),
            AnswerText.Of(Values(harness.FlattenSort())));
    }

    [Fact]
    public void MergeByHeap_TiledValueLists_AgreesWithFlattenSort()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Enumerable.Range(0, ExpectedMergedValueCount)),
            AnswerText.Of(Values(harness.MergeByHeap())));
        Assert.Equal(
            AnswerText.Of(Values(harness.FlattenSort())),
            AnswerText.Of(Values(harness.MergeByHeap())));
    }

    // Both arms report the merged chain as that internal node type through an object, so the test reads
    // the values off it rather than comparing nodes.
    private static List<int> Values(object? head)
    {
        var values = new List<int>();

        for (var node = head as SinglyLinkedListNode<int>; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static MergeKSortedListsBenchmarks BuildHarness()
    {
        var harness = new MergeKSortedListsBenchmarks { ListCount = SmallestListCount };
        harness.Setup();

        return harness;
    }
}
