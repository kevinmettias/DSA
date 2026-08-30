namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a genetic-mutation scenario shaped like LeetCode's Minimum Genetic
// Mutation (LC 433): a chain of geneCount single-character mutations starting
// from a fixed startGene, so a real shortest mutation sequence to the chain's
// last gene always exists and every benchmark below has to do genuine BFS work
// instead of failing fast. The full pairwise one-character-apart scan run
// afterward (matching MinimumGeneticMutationTests' own BuildGraph) then finds
// every edge, not just the ones the chain happened to walk - the resulting
// graph is a random cluster in the Hamming graph over {A,C,G,T}^geneLength, not
// just one bare path. Mirrors WordLadderGraphs, narrowed to the 4-letter DNA
// alphabet LC 433 actually uses.
internal static class GeneMutationGraphs
{
    private const string Alphabet = "ACGT";

    public static (List<string> Genes, string StartGene, string EndGene) BuildChain(
        int geneCount, int geneLength, int seed)
    {
        var random = new Random(seed);
        var current = new string('A', geneLength).ToCharArray();
        var genes = new List<string> { new(current) };

        for (var i = 1; i < geneCount; i++)
        {
            var position = random.Next(geneLength);
            current[position] = Alphabet[random.Next(Alphabet.Length)];
            genes.Add(new string(current));
        }

        return (genes, genes[0], genes[^1]);
    }

    public static (Dictionary<string, GeneMutationNode> NodesByGene, GeneMutationNode StartNode) BuildGraph(
        IReadOnlyList<string> genes, string startGene)
    {
        var nodesByGene = new Dictionary<string, GeneMutationNode>();

        foreach (var gene in genes)
        {
            nodesByGene.TryAdd(gene, new GeneMutationNode(gene));
        }

        var allNodes = nodesByGene.Values.ToList();

        for (var i = 0; i < allNodes.Count; i++)
        {
            for (var j = i + 1; j < allNodes.Count; j++)
            {
                if (IsOneCharApart(allNodes[i].Gene, allNodes[j].Gene))
                {
                    allNodes[i].Neighbors.Add(allNodes[j]);
                    allNodes[j].Neighbors.Add(allNodes[i]);
                }
            }
        }

        return (nodesByGene, nodesByGene[startGene]);
    }

    private static bool IsOneCharApart(string a, string b)
    {
        var differences = 0;

        for (var i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i] && ++differences > 1)
            {
                return false;
            }
        }

        return differences == 1;
    }
}
