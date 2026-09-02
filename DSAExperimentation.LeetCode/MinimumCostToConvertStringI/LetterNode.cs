namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringI;

// One node per lowercase letter; Edges holds every direct x -> y conversion
// rule (original[i], changed[i], cost[i]) out of it. Answers LC 2976 alone -
// the same problem-local weighted-node shape
// NumberOfPossibleSetsOfClosingBranches' BranchNode already establishes for a
// different problem's graph.
internal sealed class LetterNode(char letter)
{
    public char Letter { get; } = letter;

    public List<(int Weight, LetterNode Target)> Edges { get; } = [];

    public override string ToString() => Letter.ToString();
}
