namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringI;

// All 26 lowercase-letter nodes, built once - the domain model, not an
// answer to any one query about it, the same split BranchNetwork uses
// between "what the puzzle looks like" and "what a caller wants to find
// out". Duplicate rules for the same (original, changed) pair are kept as
// separate edges rather than pre-reduced to their minimum -
// AllPairsShortestPaths already takes the cheaper one on relax.
internal sealed class LetterNetwork
{
    // One node per lowercase English letter, the alphabet the conversion rules run over.
    private const int AlphabetSize = 26;

    // Index i is the node for letter 'a' + i.
    public LetterNode[] Nodes { get; }

    private LetterNetwork(LetterNode[] nodes) => Nodes = nodes;

    public static LetterNetwork Build(char[] original, char[] changed, int[] cost)
    {
        var nodes = new LetterNode[AlphabetSize];

        for (var i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new LetterNode((char)('a' + i));
        }

        for (var i = 0; i < original.Length; i++)
        {
            nodes[original[i] - 'a'].Edges.Add((cost[i], nodes[changed[i] - 'a']));
        }

        return new LetterNetwork(nodes);
    }
}
