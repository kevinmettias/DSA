namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPathWithDifferentAdjacentCharacters.Fixtures;

// parent[] edges point parent -> child, the same direction RoomNode's Children does
// for LC 1916's prevRoom[] - node 0 is always the root since parent[0] == -1 is this
// problem's own precondition.
internal sealed class PathNode(int id, char label)
{
    public int Id { get; } = id;

    public char Label { get; } = label;

    public List<PathNode> Children { get; } = [];

    public override string ToString() => Id.ToString();
}
