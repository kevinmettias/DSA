using DSAExperimentation.Algorithms.ShortestPaths.Hamming;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumGeneticMutation;

// LeetCode 433. Minimum Genetic Mutation: fewest single-character mutations from
// startGene to endGene, where every intermediate gene must be in the bank.
//
// Structurally identical to Word Ladder (127) over the 4-letter DNA alphabet
// instead of 26 letters, and answered in edges walked rather than nodes visited -
// so it shares DataStructures.Graph.Hamming outright and differs only in the "+1" WordLadder
// applies and the alphabet it mutates over.
internal static class MinimumGeneticMutationSolution
{
    // The textbook answer: BCL Queue + HashSet over candidates generated on the
    // fly, never materializing the graph.
    public static int MinMutationByMutationQueue(string startGene, string endGene, IEnumerable<string> bank)
    {
        var bankSet = new Set<string>(bank);

        return MinMutationByMutationQueue(startGene, endGene, bankSet);
    }

    public static int MinMutationByMutationQueue(string startGene, string endGene, Set<string> bank)
    {
        // The graph strategy seeds startGene itself, so an endGene equal to it is
        // reachable even when the bank omits it - matched here for parity.
        if (!bank.Has(endGene) && endGene != startGene)
        {
            return LeetCodeAnswer.None;
        }

        return HammingSearch.MutationDistance(startGene, endGene, bank, StandardAlphabets.Dna) ?? LeetCodeAnswer.None;
    }

    // This repo's own BFS over the materialized bank graph.
    public static int MinMutationByReduceGraph(string startGene, string endGene, IEnumerable<string> bank)
    {
        var graph = HammingGraph.Build(startGene, bank);

        return MinMutationByReduceGraph(graph, endGene);
    }

    public static int MinMutationByReduceGraph(HammingGraph graph, string endGene)
    {
        if (!graph.TryGetNode(endGene, out var endNode))
        {
            return LeetCodeAnswer.None;
        }

        var distances = HammingDistances.From(graph.Root);

        return distances.TryGetValue(endNode, out var distance) ? distance : LeetCodeAnswer.None;
    }
}
