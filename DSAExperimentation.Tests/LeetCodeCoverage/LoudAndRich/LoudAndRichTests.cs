using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.LoudAndRich.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LoudAndRich;

// LeetCode 851. Loud and Rich: richer[i] = [a, b] means a definitely has more
// money than b, so a "poorer than" edge a -> b turns the whole richer relation
// into a DAG this repo's own TopologicalSort.TrySort can order richest-first
// (Kahn's algorithm - in-degree 0 means "no one confirmed richer yet"). One
// linear DP pass over that order then propagates the quietest person seen so
// far down every "poorer than" edge, the canonical O(V+E) solution instead of
// re-walking each person's own reachable set from scratch.
public sealed partial class LoudAndRichTests
{
    [Fact]
    public void LoudAndRich_ClassicExample_ReturnsQuietestRicherOrEqualPersonPerPerson()
    {
        int[][] richer = [[1, 0], [2, 1], [3, 1], [3, 7], [4, 3], [5, 3], [6, 3]];
        int[] quiet = [3, 2, 5, 4, 6, 1, 7, 0];

        var answer = LoudAndRich(richer, quiet);

        Assert.Equal([5, 5, 2, 5, 4, 5, 6, 7], answer);
    }

    [Fact]
    public void LoudAndRich_NoRicherRelations_EveryoneIsTheirOwnAnswer()
    {
        int[][] richer = [];
        int[] quiet = [4, 1, 3];

        var answer = LoudAndRich(richer, quiet);

        Assert.Equal([0, 1, 2], answer);
    }

    private static int[] LoudAndRich(int[][] richer, int[] quiet)
    {
        var nodes = BuildPersonNodes(richer, quiet.Length);

        TopologicalSort.TrySort<
            PersonNode, PersonTopology, ListChildren<PersonNode>,
            NaturalChildOrder<PersonNode, ListChildren<PersonNode>>, ListChildren<PersonNode>>(
            nodes, out var ordering);

        var answer = Enumerable.Range(0, quiet.Length).ToArray();

        foreach (var node in ordering)
        {
            foreach (var poorer in node.Poorer)
            {
                if (quiet[answer[node.Id]] < quiet[answer[poorer.Id]])
                {
                    answer[poorer.Id] = answer[node.Id];
                }
            }
        }

        return answer;
    }

    private static PersonNode[] BuildPersonNodes(int[][] richer, int personCount)
    {
        var nodes = new PersonNode[personCount];

        for (var i = 0; i < personCount; i++)
        {
            nodes[i] = new PersonNode(i);
        }

        foreach (var pair in richer)
        {
            nodes[pair[0]].Poorer.Add(nodes[pair[1]]);
        }

        return nodes;
    }
}
