using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// An edge of the suffix tree is the range of the text its node names: "nan", from 2 for 3, in
// "banana".
public sealed partial class SuffixTreeEdgesTests
{
    private const string Text = "banana";
    private const int NanStart = 2;
    private const int NanLength = 3;

    [Fact]
    public void LengthOf_IsTheNodesRangeLength() =>
        Assert.Equal(NanLength, new SuffixTreeEdges(Text).LengthOf(NanEdge()));

    [Fact]
    public void HasLetterAt_ReadsTheTextFromTheRangesStart()
    {
        var edges = new SuffixTreeEdges(Text);
        var node = NanEdge();

        Assert.All(
            Enumerable.Range(0, NanLength),
            offset => Assert.True(edges.HasLetterAt(node, offset, Text[NanStart + offset])));
    }

    [Fact]
    public void HasLetterAt_AnotherLetter_IsFalse() =>
        Assert.False(new SuffixTreeEdges(Text).HasLetterAt(NanEdge(), 1, 'n'));

    private static SuffixTreeNode NanEdge() => new() { Start = NanStart, Length = NanLength };
}
