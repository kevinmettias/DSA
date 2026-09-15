using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.LeetCode.OpenTheLock;

// LeetCode 752. Open the Lock: fewest wheel turns from "0000" to a target
// combination without ever passing through a deadend.
//
// The puzzle is a shortest-path query on Domain.Locks' wheel-turn graph, so the
// only thing this class contributes is the start combination and the choice of
// how to search.
internal static class OpenTheLockSolution
{
    // The textbook answer: BCL Queue + HashSet, generating each of the 8 candidate
    // turns on the fly and never materializing the graph. Deliberately written
    // without this repo's primitives - it is the arm the composed solution below
    // has to justify itself against.
    public static int MinTurnsByMutationQueue(IEnumerable<string> deadends, string target)
    {
        var deadendSet = new Set<string>(deadends);

        return MinTurnsByMutationQueue(deadendSet, target);
    }

    public static int MinTurnsByMutationQueue(Set<string> deadends, string target)
    {
        if (deadends.Has(LockStart.Combination))
        {
            return LeetCodeAnswer.None;
        }

        var walk = new TurnWalk(deadends, [LockStart.Combination], new Queue<(string, int)>());
        walk.Queue.Enqueue((LockStart.Combination, 0));

        while (walk.Queue.Count > 0)
        {
            var (combination, turns) = walk.Queue.Dequeue();

            if (combination == target)
            {
                return turns;
            }

            EnqueueOpenNeighbors(combination, turns, walk);
        }

        return LeetCodeAnswer.None;
    }

    private static void EnqueueOpenNeighbors(string combination, int turns, TurnWalk walk)
    {
        foreach (var neighbor in LockGraph.WheelTurnNeighbors(combination))
        {
            if (!walk.Deadends.Has(neighbor) && walk.Visited.Add(neighbor))
            {
                walk.Queue.Enqueue((neighbor, turns + 1));
            }
        }
    }

    private readonly record struct TurnWalk(
        Set<string> Deadends,
        HashSet<string> Visited,
        Queue<(string Combination, int Turns)> Queue);

    // This repo's own BFS: Reduce.Graph in BreadthFirstReduceOrder with
    // DistanceMapReduceAlgebra is already exactly "distance from a root to every
    // node", so the puzzle reduces to one lookup in the result - the same
    // composition WordLadder uses for LC 127.
    public static int MinTurnsByReduceGraph(IEnumerable<string> deadends, string target)
    {
        var graph = LockGraph.Build(deadends);

        return MinTurnsByReduceGraph(graph, target);
    }

    public static int MinTurnsByReduceGraph(LockGraph graph, string target)
    {
        if (!graph.TryGetNode(LockStart.Combination, out var startNode) ||
            !graph.TryGetNode(target, out var targetNode))
        {
            return LeetCodeAnswer.None;
        }

        var distances = Reduce.Graph<
            LockNode, LockTopology, ListChildren<LockNode>,
            NaturalChildOrder<LockNode, ListChildren<LockNode>>, ListChildren<LockNode>,
            BreadthFirstReduceOrder<LockNode>,
            DistanceMapReduceAlgebra<LockNode>, Dictionary<LockNode, int>>(startNode);

        return distances.TryGetValue(targetNode, out var distance) ? distance : LeetCodeAnswer.None;
    }
}
