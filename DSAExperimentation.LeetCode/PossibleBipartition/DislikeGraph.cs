namespace DSAExperimentation.LeetCode.PossibleBipartition;

// LC 886's (personCount, dislikes) input materialized as PersonNode objects.
// Dislike is mutual, so every listed pair is wired in both directions - which is
// exactly the symmetric-adjacency precondition
// Algorithms.Bipartiteness.BipartiteCheck documents.
//
// This exists as a type rather than a bare node list so a benchmark can hoist
// construction into [GlobalSetup] and hand the prepared graph to the strategy's
// second overload without that overload becoming ambiguous with the
// LeetCode-shaped one (ARCHITECTURE.md #17.4).
internal sealed class DislikeGraph
{
    // Every person, including those nobody dislikes: BipartiteCheck is a
    // multi-root walk and only visits the components its roots reach.
    public IReadOnlyList<PersonNode> People { get; }

    private DislikeGraph(IReadOnlyList<PersonNode> people) => People = people;

    // A PersonNode per person id, then both directions of every dislike pair.
    // LeetCodeAdjacency states that layout once for every problem taking an (n, edges)
    // pair; slot i holding person i is this problem being numbered 1..personCount,
    // and the placeholder that numbering leaves at slot 0 is sliced off below.
    public static DislikeGraph Build(int personCount, int[][] dislikes)
    {
        var byId = LeetCodeAdjacency.OneBased<PersonNode>(
            personCount, dislikes, id => new PersonNode(id), (person, _, farPerson, _) => person.Dislikes.Add(farPerson));

        return new DislikeGraph(byId[PersonNumbering.First..]);
    }
}
