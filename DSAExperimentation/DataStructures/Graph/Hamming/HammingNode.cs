namespace DSAExperimentation.DataStructures.Graph.Hamming;

// One node per distinct string in a Hamming graph; Neighbors holds every other
// string in the graph exactly one character apart, filled in once while the graph
// is built.
internal sealed class HammingNode(string value)
{
    public string Value { get; } = value;

    public List<HammingNode> Neighbors { get; } = [];

    public override string ToString() => Value;
}
