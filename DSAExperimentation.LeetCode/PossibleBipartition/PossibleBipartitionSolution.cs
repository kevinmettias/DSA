using DSAExperimentation.Algorithms.Bipartiteness;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.PossibleBipartition;

// LeetCode 886. Possible Bipartition: personCount people numbered
// 1..personCount and a list of mutual dislike pairs - can they be split into two
// groups so that no dislike pair ends up sharing one?
//
// That is 2-colorability of the dislikes graph, so both strategies are a greedy
// coloring walk that fails the moment an edge joins two same-colored people. The
// graph may be disconnected (and people nobody dislikes are isolated vertices),
// so both restart from every still-uncolored person.
internal static class PossibleBipartitionSolution
{
    // Uncolored; the two groups are +1 and -1 so switching groups is a negation.
    private const sbyte Uncolored = 0;
    private const sbyte FirstGroup = 1;

    // The textbook answer: an sbyte[] group array keyed by person id and an
    // explicit Stack<int> walking the dislikes adjacency iteratively, depth
    // first. Deliberately written without this repo's primitives - it is the arm
    // the composed solution below has to justify itself against.
    public static bool CanBipartitionByColorArrayDfs(int personCount, int[][] dislikes)
    {
        var adjacency = DislikeAdjacency.Build(personCount, dislikes);
        return CanBipartitionByColorArrayDfs(adjacency);
    }

    public static bool CanBipartitionByColorArrayDfs(DislikeAdjacency adjacency)
    {
        var neighbors = adjacency.Neighbors;
        var group = new sbyte[neighbors.Length];
        var stack = new Stack<int>();

        for (var start = PersonNumbering.First; start < neighbors.Length; start++)
        {
            if (group[start] != Uncolored)
            {
                continue;
            }

            group[start] = FirstGroup;
            stack.Push(start);

            if (!TrySplitComponent(neighbors, stack, group))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TrySplitComponent(int[][] neighbors, Stack<int> stack, sbyte[] group)
    {
        while (stack.Count > 0)
        {
            var person = stack.Pop();

            if (!TrySplitDisliked(neighbors, stack, group, person))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TrySplitDisliked(int[][] neighbors, Stack<int> stack, sbyte[] group, int person)
    {
        foreach (var disliked in neighbors[person])
        {
            if (group[disliked] == group[person])
            {
                return false;
            }

            if (group[disliked] == Uncolored)
            {
                group[disliked] = (sbyte)-group[person];
                stack.Push(disliked);
            }
        }

        return true;
    }

    // This repo's own answer: Algorithms.Bipartiteness.BipartiteCheck is already a
    // multi-root BFS 2-coloring over any IGraphTopology, so the problem reduces to
    // materializing the dislikes as a PersonNode graph and asking it - the same
    // composition IsGraphBipartiteSolution uses for LC 785, just fed from
    // 1-indexed people and a pair list instead of an adjacency list.
    public static bool CanBipartitionByBipartiteCheck(int personCount, int[][] dislikes)
    {
        var graph = DislikeGraph.Build(personCount, dislikes);
        return CanBipartitionByBipartiteCheck(graph);
    }

    public static bool CanBipartitionByBipartiteCheck(DislikeGraph graph) =>
        BipartiteCheck.IsBipartite<
            PersonNode, PersonTopology, ListChildren<PersonNode>,
            NaturalChildOrder<PersonNode, ListChildren<PersonNode>>, ListChildren<PersonNode>>(
            graph.People);
}
