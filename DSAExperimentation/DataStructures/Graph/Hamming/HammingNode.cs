namespace DSAExperimentation.DataStructures.Graph.Hamming;

// One node per distinct string in a Hamming graph; Neighbors holds every other
// string in the graph exactly one character apart, filled in once while the graph
// is built.
/// <summary>A node of a <see cref="HammingGraph"/>, standing for one distinct string.</summary>
/// <param name="value">The string the node stands for.</param>
public sealed class HammingNode(string value)
{
    /// <summary>The string the node stands for.</summary>
    public string Value { get; } = value;

    /// <summary>The nodes whose strings differ from this one's in exactly one position.</summary>
    public List<HammingNode> Neighbors { get; } = [];

    public override string ToString() => Value;
}
