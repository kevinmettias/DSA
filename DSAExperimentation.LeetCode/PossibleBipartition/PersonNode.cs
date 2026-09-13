namespace DSAExperimentation.LeetCode.PossibleBipartition;

// One person in LC 886's dislikes graph, with Dislikes holding every other person
// they refuse to share a group with. Answers LC 886 alone - a plain
// adjacency-list graph node with no fixed vertex set or modulus, so it lives
// beside the solution rather than in Domain/ (ARCHITECTURE.md #17.6), exactly
// like IsGraphBipartite's BipartiteNode.
internal sealed class PersonNode(int id)
{
    public int Id { get; } = id;

    public List<PersonNode> Dislikes { get; } = [];

    public override string ToString() => Id.ToString();
}
