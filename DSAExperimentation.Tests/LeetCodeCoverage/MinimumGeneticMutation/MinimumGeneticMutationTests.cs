using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.Tests.LeetCodeCoverage.MinimumGeneticMutation.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumGeneticMutation;

// LeetCode 433. Minimum Genetic Mutation: genes are nodes of an implicit graph,
// with an edge between any two genes differing by exactly one of the 8
// characters - the exact same "implicit graph + BFS shortest distance" shape
// WordLadderTests already proves via Reduce.Graph's own DistanceMapReduceAlgebra,
// just without WordLadder's "+1" (LeetCode counts mutations here as edges walked,
// not words/genes in the sequence).
public sealed partial class MinimumGeneticMutationTests
{
    [Fact]
    public void MinMutation_ClassicExample_ReturnsOne()
    {
        var mutations = MinMutation("AACCGGTT", "AACCGGTA", ["AACCGGTA"]);

        Assert.Equal(1, mutations);
    }

    [Fact]
    public void MinMutation_EndGeneMissingFromBank_ReturnsNegativeOne()
    {
        var mutations = MinMutation("AACCGGTT", "AAACGGTA", ["AACCGGTA", "AACCGCTA"]);

        Assert.Equal(-1, mutations);
    }

    private static int MinMutation(string startGene, string endGene, string[] bank)
    {
        var nodesByGene = BuildGraph(startGene, bank, out var startNode);

        if (!nodesByGene.TryGetValue(endGene, out var endNode))
        {
            return -1;
        }

        if (startGene == endGene)
        {
            return 0;
        }

        var distances = Reduce.Graph<
            GeneNode, GeneTopology, ListChildren<GeneNode>,
            NaturalChildOrder<GeneNode, ListChildren<GeneNode>>, ListChildren<GeneNode>,
            BreadthFirstReduceOrder<GeneNode>,
            DistanceMapReduceAlgebra<GeneNode>, Dictionary<GeneNode, int>>(startNode);

        return distances.TryGetValue(endNode, out var distance) ? distance : -1;
    }

    // An edge joins any two genes differing by exactly one of the 8 characters -
    // the same small-n O(n^2 * L) pairwise scan WordLadderTests.BuildGraph already
    // uses, for the same reason (see its own comment).
    private static HashMap<string, GeneNode> BuildGraph(string startGene, string[] bank, out GeneNode startNode)
    {
        var nodesByGene = new HashMap<string, GeneNode>();
        startNode = new GeneNode(startGene);
        nodesByGene.Set(startGene, startNode);

        foreach (var gene in bank)
        {
            if (!nodesByGene.HasKey(gene))
            {
                nodesByGene.Set(gene, new GeneNode(gene));
            }
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

        return nodesByGene;
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
