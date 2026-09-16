using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SubtreeOfAnotherTreeWorkloads (ARCHITECTURE 17.7). The LC 572 reading
// depends on the chain being left-skewed and on every node but the last sharing the same filler
// value: the naive per-node comparison then has to walk deep into every candidate start before
// failing, and the one distinct value at the deepest node is what stops it from ever finding a
// real match and short-circuiting.
public sealed partial class SubtreeOfAnotherTreeWorkloadsTests
{
    private const int Length = 200;
    private const int FillerValue = 1;
    private const int DistinctLastValue = -1;

    [Fact]
    public void BuildLeftChain_Length_ReturnsOneNodePerPosition() =>
        Assert.Equal(Length, Nodes(SubtreeOfAnotherTreeWorkloads.BuildLeftChain(Length, DistinctLastValue)).Count);

    [Fact]
    public void BuildLeftChain_EveryNode_HasNoRightChild() =>
        Assert.All(
            Nodes(SubtreeOfAnotherTreeWorkloads.BuildLeftChain(Length, DistinctLastValue)),
            node => Assert.Null(node.Right));

    [Fact]
    public void BuildLeftChain_EveryNodeButTheLast_CarriesTheSharedFillerValue()
    {
        var values = Nodes(SubtreeOfAnotherTreeWorkloads.BuildLeftChain(Length, DistinctLastValue))
            .Select(node => node.Value)
            .ToList();

        Assert.All(values[..^1], value => Assert.Equal(FillerValue, value));
    }

    [Fact]
    public void BuildLeftChain_LastNode_CarriesTheValueTheLargerTreeNeverRepeats()
    {
        var values = Nodes(SubtreeOfAnotherTreeWorkloads.BuildLeftChain(Length, DistinctLastValue))
            .Select(node => node.Value)
            .ToList();

        Assert.Equal(DistinctLastValue, values[^1]);
        Assert.DoesNotContain(DistinctLastValue, values[..^1]);
    }

    [Fact]
    public void BuildLeftChain_SingleNode_CarriesTheLastValueAtItsRoot()
    {
        var root = SubtreeOfAnotherTreeWorkloads.BuildLeftChain(1, DistinctLastValue);

        Assert.Equal(DistinctLastValue, root.Value);
        Assert.Null(root.Left);
    }

    [Fact]
    public void BuildLeftChain_SameLengthAndValue_ReturnsTheSameChain()
    {
        var chain = Nodes(SubtreeOfAnotherTreeWorkloads.BuildLeftChain(Length, DistinctLastValue));
        var repeat = Nodes(SubtreeOfAnotherTreeWorkloads.BuildLeftChain(Length, DistinctLastValue));

        Assert.Equal(AnswerText.Of(chain.Select(node => node.Value)), AnswerText.Of(repeat.Select(node => node.Value)));
    }

    // Walks the left spine itself rather than reusing any traversal the benchmark owns, so the
    // shape assertions above are about the chain, not about how it was built.
    private static List<BinaryTreeNode<int>> Nodes(BinaryTreeNode<int> root)
    {
        var nodes = new List<BinaryTreeNode<int>>();
        var current = root;

        while (current is not null)
        {
            nodes.Add(current);
            current = current.Left;
        }

        return nodes;
    }
}
