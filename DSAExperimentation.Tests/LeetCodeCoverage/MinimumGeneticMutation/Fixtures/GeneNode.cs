namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumGeneticMutation.Fixtures;

// One node per candidate gene; Neighbors holds every other gene in the bank
// exactly one character apart, filled in once while the graph is built.
internal sealed class GeneNode(string gene)
{
    public string Gene { get; } = gene;

    public List<GeneNode> Neighbors { get; } = [];

    public override string ToString() => Gene;
}
