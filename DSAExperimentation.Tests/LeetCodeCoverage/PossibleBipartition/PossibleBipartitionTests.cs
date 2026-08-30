using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.PossibleBipartition.Fixtures;
using BipartiteCheckOperations = DSAExperimentation.Algorithms.Bipartiteness.BipartiteCheck;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PossibleBipartition;

// LeetCode 886. Possible Bipartition: n people and a list of mutual-dislike
// pairs - can they be split into two groups with no dislike pair sharing a
// group? Exactly this repo's own BipartiteCheck.IsBipartite over the
// dislikes graph (edges are already symmetric, matching BipartiteCheck's
// own precondition) - the same primitive Is Graph Bipartite? (LC 785)
// composes, just fed from 1-indexed people and a pair-list input instead of
// an adjacency list.
public sealed partial class PossibleBipartitionTests
{
    [Fact]
    public void PossibleBipartition_ClassicExample_ReturnsTrue()
    {
        var people = BuildGraph(n: 4, dislikes: [[1, 2], [1, 3], [2, 4]]);

        Assert.True(Check(people));
    }

    [Fact]
    public void PossibleBipartition_TriangleOfDislikes_ReturnsFalse()
    {
        var people = BuildGraph(n: 3, dislikes: [[1, 2], [1, 3], [2, 3]]);

        Assert.False(Check(people));
    }

    private static List<PersonNode> BuildGraph(int n, int[][] dislikes)
    {
        // Index 0 is an unused placeholder so person ids can stay 1-indexed,
        // matching LeetCode's own numbering; it carries no edges and is
        // dropped before the graph is returned.
        var people = Enumerable.Range(0, n + 1).Select(id => new PersonNode(id)).ToList();

        foreach (var pair in dislikes)
        {
            people[pair[0]].Dislikes.Add(people[pair[1]]);
            people[pair[1]].Dislikes.Add(people[pair[0]]);
        }

        return people.Skip(1).ToList();
    }

    private static bool Check(List<PersonNode> people)
        => BipartiteCheckOperations.IsBipartite<
            PersonNode, PersonTopology, ListChildren<PersonNode>,
            NaturalChildOrder<PersonNode, ListChildren<PersonNode>>, ListChildren<PersonNode>>(
            people);
}
