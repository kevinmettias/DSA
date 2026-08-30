namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' WordLadder(II) WordNode fixture: one node per
// candidate word, Neighbors holding every other word in the graph exactly one
// letter apart.
internal sealed class WordLadderNode(string word)
{
    public string Word { get; } = word;

    public List<WordLadderNode> Neighbors { get; } = [];
}
