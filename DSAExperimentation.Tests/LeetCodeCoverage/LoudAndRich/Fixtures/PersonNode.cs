namespace DSAExperimentation.Tests.LeetCodeCoverage.LoudAndRich.Fixtures;

// One node per person; Poorer holds every other person known to have strictly
// less money, filled in once while the graph is built from LeetCode's `richer`
// pairs - the direction TopologicalSort.TrySort needs to process richest-to-
// poorest before the answer DP pass runs.
internal sealed class PersonNode(int id)
{
    public int Id { get; } = id;

    public List<PersonNode> Poorer { get; } = [];

    public override string ToString() => $"Person({Id})";
}
