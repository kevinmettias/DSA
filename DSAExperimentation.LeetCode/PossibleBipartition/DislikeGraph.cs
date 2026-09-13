namespace DSAExperimentation.LeetCode.PossibleBipartition;

// LC 886's (n, dislikes) input materialized as PersonNode objects. Dislike is
// mutual, so every listed pair is wired in both directions - which is exactly the
// symmetric-adjacency precondition Algorithms.Bipartiteness.BipartiteCheck
// documents.
//
// This exists as a type rather than a bare node list so a benchmark can hoist
// construction into [GlobalSetup] and hand the prepared graph to the strategy's
// second overload without that overload becoming ambiguous with the
// LeetCode-shaped one (ARCHITECTURE.md #17.4).
internal sealed class DislikeGraph
{
    private DislikeGraph(IReadOnlyList<PersonNode> people) => People = people;

    // Every person, including those nobody dislikes: BipartiteCheck is a
    // multi-root walk and only visits the components its roots reach.
    public IReadOnlyList<PersonNode> People { get; }

    public static DislikeGraph Build(int n, int[][] dislikes)
    {
        // Slot i holds person i, so LeetCode's 1..n numbering indexes directly;
        // slot 0 is a placeholder that carries no edges and is not returned.
        var byId = new PersonNode[n + 1];

        for (var id = PossibleBipartitionSolution.FirstPerson; id <= n; id++)
        {
            byId[id] = new PersonNode(id);
        }

        foreach (var pair in dislikes)
        {
            byId[pair[0]].Dislikes.Add(byId[pair[1]]);
            byId[pair[1]].Dislikes.Add(byId[pair[0]]);
        }

        return new DislikeGraph(byId[PossibleBipartitionSolution.FirstPerson..]);
    }
}
