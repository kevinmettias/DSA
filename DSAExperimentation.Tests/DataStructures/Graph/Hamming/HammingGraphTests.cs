using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Tests.DataStructures.Graph.Hamming;

public sealed class HammingGraphTests
{
    public static TheoryData<OneApartExample> IsOneApartExamples =>
        new()
        {
            { new OneApartExample(First: "abc", Second: "abd", Expected: true) },
            { new OneApartExample(First: "abc", Second: "abc", Expected: false) },
            { new OneApartExample(First: "abc", Second: "axd", Expected: false) },
            { new OneApartExample(First: "a", Second: "b", Expected: true) },
        };

    [Fact]
    public void Build_SeedsTheRootEvenWhenTheValueSetOmitsIt()
    {
        var graph = HammingGraph.Build("hit", ["hot", "dot"]);
        var found = graph.TryGetNode("hit", out var root);

        Assert.True(found);
        Assert.Same(root, graph.Root);
    }

    [Fact]
    public void Build_JoinsExactlyThePairsOneCharacterApart()
    {
        var graph = HammingGraph.Build("hit", ["hot", "dot", "dog"]);
        var found = graph.TryGetNode("hot", out var hot);

        Assert.True(found);

        // hit-hot and hot-dot are one apart; hot-dog differs in two positions.
        Assert.Equal(["dot", "hit"], hot.Neighbors.Select(n => n.Value).OrderBy(v => v));
    }

    [Fact]
    public void Build_WiresEveryEdgeInBothDirections()
    {
        var graph = HammingGraph.Build("aa", ["ab"]);

        AssertBothValuesBecameNodes(graph);
        AssertEachNodeReachesTheOther(graph);
    }

    [Fact]
    public void Build_DeduplicatesRepeatedValues()
    {
        var graph = HammingGraph.Build("aa", ["ab", "ab", "aa"]);
        var found = graph.TryGetNode("ab", out var node);

        Assert.True(found);
        Assert.Single(node.Neighbors);
    }

    [Fact]
    public void TryGetNode_ValueNotInTheGraph_ReturnsFalse()
    {
        var graph = HammingGraph.Build("aa", ["ab"]);
        var found = graph.TryGetNode("zz", out _);

        Assert.False(found);
    }

    [Theory]
    [MemberData(nameof(IsOneApartExamples))]
    public void IsOneApart_ComparesEqualLengthStrings_ReturnsTrueOnlyForASingleDifference(OneApartExample example)
    {
        var oneApart = HammingGraph.IsOneApart(example.First, example.Second);

        Assert.Equal(example.Expected, oneApart);
    }

    [Fact]
    public void IsOneApart_IsSymmetric()
    {
        var forward = HammingGraph.IsOneApart("abc", "abd");
        var backward = HammingGraph.IsOneApart("abd", "abc");

        Assert.Equal(forward, backward);
    }

    [Fact]
    public void OneCharacterMutations_ProducesEveryReplacementAndNeverTheOriginal()
    {
        var mutations = HammingGraph.OneCharacterMutations("ab", new Alphabet("abc")).ToList();
        var sorted = mutations.OrderBy(m => m, StringComparer.Ordinal);

        Assert.Equal(["aa", "ac", "bb", "cb"], sorted);
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

        Assert.All(mutations, m =>
        {
            var oneApart = HammingGraph.IsOneApart("aaa", m);

            Assert.True(oneApart);
        });
    }

    private static void AssertBothValuesBecameNodes(HammingGraph graph)
    {
        var foundFirst = graph.TryGetNode("aa", out _);
        var foundSecond = graph.TryGetNode("ab", out _);

        Assert.True(foundFirst);
        Assert.True(foundSecond);
    }

    private static void AssertEachNodeReachesTheOther(HammingGraph graph)
    {
        graph.TryGetNode("aa", out var first);
        graph.TryGetNode("ab", out var second);

        Assert.Contains(second, first.Neighbors);
        Assert.Contains(first, second.Neighbors);
    }

    // One example pair: two equal-length strings and whether they differ in exactly
    // one position. Named fields rather than two adjacent `string` positions, so the
    // row states which value it is talking about.
    public readonly record struct OneApartExample(string First, string Second, bool Expected);
}
