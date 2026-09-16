using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeNodesInBetweenZerosBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - buffering every value into a list and bucketing it into
// group sums against appending each group's sum on a single pass - so a harness whose arms disagree is
// timing two different problems. Both arms report the merged chain as an object over the data
// structure's internal node type, so the test reads the values off it rather than comparing nodes.
//
// The generated list is a leading zero, GroupCount runs of one to four non-zero values each closed by
// a zero, so the answer is documented to be exactly one sum node per run - GroupCount nodes, none of
// them the delimiter. Neither arm mutates the chain it is handed, so the shared prepared list makes one
// harness safe to read twice in either order; Setup draws the runs from one fixed seed, so the same
// GroupCount must rebuild the same chain.
public sealed partial class MergeNodesInBetweenZerosBenchmarksTests
{
    private const int SmallestGroupCount = 200;

    private const int ExpectedMergedNodeCount = SmallestGroupCount;

    [Fact]
    public void Setup_SameGroupCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(Values(BuildHarness().TwoPassValueBuffer())),
            AnswerText.Of(Values(BuildHarness().TwoPassValueBuffer())));

    [Fact]
    public void TwoPassValueBuffer_DelimitedSeededList_AgreesWithSinglePassSum()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMergedNodeCount, Values(harness.TwoPassValueBuffer()).Count);
        Assert.Equal(
            AnswerText.Of(Values(harness.SinglePassSum())),
            AnswerText.Of(Values(harness.TwoPassValueBuffer())));
    }

    [Fact]
    public void SinglePassSum_DelimitedSeededList_AgreesWithTwoPassValueBuffer()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMergedNodeCount, Values(harness.SinglePassSum()).Count);
        Assert.Equal(
            AnswerText.Of(Values(harness.TwoPassValueBuffer())),
            AnswerText.Of(Values(harness.SinglePassSum())));
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

    private static MergeNodesInBetweenZerosBenchmarks BuildHarness()
    {
        var harness = new MergeNodesInBetweenZerosBenchmarks { GroupCount = SmallestGroupCount };
        harness.Setup();

        return harness;
    }
}
