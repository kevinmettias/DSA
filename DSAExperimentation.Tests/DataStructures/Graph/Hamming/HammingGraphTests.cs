using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Tests.DataStructures.Graph.Hamming;

public sealed class HammingGraphTests
{
    [Fact]
    public void Build_SeedsTheRootEvenWhenTheValueSetOmitsIt()
    {
        var graph = HammingGraph.Build("hit", ["hot", "dot"]);

        Assert.True(graph.TryGetNode("hit", out var root));
        Assert.Same(root, graph.Root);
    }

    [Fact]
    public void Build_JoinsExactlyThePairsOneCharacterApart()
    {
        var graph = HammingGraph.Build("hit", ["hot", "dot", "dog"]);

        Assert.True(graph.TryGetNode("hot", out var hot));

        // hit-hot and hot-dot are one apart; hot-dog differs in two positions.
        Assert.Equal(["dot", "hit"], hot.Neighbors.Select(n => n.Value).OrderBy(v => v));
    }

    [Fact]
    public void Build_WiresEveryEdgeInBothDirections()
    {
        var graph = HammingGraph.Build("aa", ["ab"]);

        Assert.True(graph.TryGetNode("aa", out var first));
        Assert.True(graph.TryGetNode("ab", out var second));

        Assert.Contains(second, first.Neighbors);
        Assert.Contains(first, second.Neighbors);
    }

    [Fact]
    public void Build_DeduplicatesRepeatedValues()
    {
        var graph = HammingGraph.Build("aa", ["ab", "ab", "aa"]);

        Assert.True(graph.TryGetNode("ab", out var node));
        Assert.Single(node.Neighbors);
    }

    [Fact]
    public void TryGetNode_ValueNotInTheGraph_ReturnsFalse()
    {
        var graph = HammingGraph.Build("aa", ["ab"]);

        Assert.False(graph.TryGetNode("zz", out _));
    }

    [Theory]
    [InlineData("abc", "abd", true)]
    [InlineData("abc", "abc", false)]
    [InlineData("abc", "axd", false)]
    [InlineData("a", "b", true)]
    public void IsOneApart_ComparesEqualLengthStrings_ReturnsTrueOnlyForASingleDifference(
        string first, string second, bool expected) =>
        Assert.Equal(expected, HammingGraph.IsOneApart(first, second));

    [Fact]
    public void IsOneApart_IsSymmetric() =>
        Assert.Equal(
            HammingGraph.IsOneApart("abc", "abd"),
            HammingGraph.IsOneApart("abd", "abc"));

    [Fact]
    public void OneCharacterMutations_ProducesEveryReplacementAndNeverTheOriginal()
    {
        var mutations = HammingGraph.OneCharacterMutations("ab", new Alphabet("abc")).ToList();

        Assert.Equal(["aa", "ac", "bb", "cb"], mutations.OrderBy(m => m, StringComparer.Ordinal));
        Assert.DoesNotContain("ab", mutations);
    }

    [Fact]
    public void OneCharacterMutations_YieldsPositionsTimesAlphabetMinusOneResults()
    {
        var overTheAlphabet = HammingGraph.OneCharacterMutations("AAAA", StandardAlphabets.Dna).ToList();

        Assert.Equal(4 * 3, overTheAlphabet.Count);

        // A source character outside the alphabet is never skipped, so that
        // position contributes a full alphabet's worth of replacements instead.
        var outsideTheAlphabet = HammingGraph.OneCharacterMutations("aAAA", StandardAlphabets.Dna).ToList();

        Assert.Equal(4 * 3 + 1, outsideTheAlphabet.Count);
    }

    [Fact]
    public void OneCharacterMutations_RestoresItsBufferBetweenPositions()
    {
        // A stale buffer would leak an earlier position's replacement into later
        // results; every mutation must differ from the source in exactly one place.
        var mutations = HammingGraph.OneCharacterMutations("aaa", new Alphabet("ab"));

        Assert.All(mutations, m => Assert.True(HammingGraph.IsOneApart("aaa", m)));
    }
}
