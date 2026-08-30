namespace DSAExperimentation.Tests.LeetCodeCoverage.WordLadderII.Fixtures;

// One node per candidate word; Neighbors holds every other word in the graph
// exactly one letter apart, filled in once while the graph is built.
internal sealed class WordNode(string word)
{
    public string Word { get; } = word;

    public List<WordNode> Neighbors { get; } = [];

    public override string ToString() => Word;
}
