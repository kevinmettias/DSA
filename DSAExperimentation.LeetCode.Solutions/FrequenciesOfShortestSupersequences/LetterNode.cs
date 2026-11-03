namespace DSAExperimentation.LeetCode.Solutions.FrequenciesOfShortestSupersequences;

// One node per non-doubled letter in a single doubled-subset candidate's induced
// subgraph - built fresh per candidate (§17.3's per-problem witness), the same
// "wire once, read via the topology" shape as tier 1's StateNode.
internal sealed class LetterNode(char letter)
{
    public char Letter { get; } = letter;

    public List<LetterNode> Neighbors { get; } = [];

    public override string ToString() => Letter.ToString();
}
