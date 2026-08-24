namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

internal sealed class WeightedNode(string name)
{
    public string Name { get; } = name;

    public List<(int Weight, WeightedNode Target)> Edges { get; } = [];

    public override string ToString() => Name;
}
