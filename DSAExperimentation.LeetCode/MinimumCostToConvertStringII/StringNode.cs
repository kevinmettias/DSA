namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

// One node per distinct string appearing in original/changed; Edges holds
// every direct x -> y substring-conversion rule out of it. Answers LC 2977
// alone - the same problem-local weighted-node shape LetterNode already
// establishes for LC 2976's single-character version.
internal sealed class StringNode(string value)
{
    public string Value { get; } = value;

    public List<(int Weight, StringNode Target)> Edges { get; } = [];

    public override string ToString() => Value;
}
