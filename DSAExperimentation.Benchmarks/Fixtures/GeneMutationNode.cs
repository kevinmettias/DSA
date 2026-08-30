namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' MinimumGeneticMutation GeneNode fixture: one
// node per candidate gene, Neighbors holding every other gene in the graph
// exactly one character apart.
internal sealed class GeneMutationNode(string gene)
{
    public string Gene { get; } = gene;

    public List<GeneMutationNode> Neighbors { get; } = [];
}
