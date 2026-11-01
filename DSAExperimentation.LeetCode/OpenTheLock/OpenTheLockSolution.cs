using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.LeetCode.OpenTheLock;

// LeetCode 752. Open the Lock: fewest wheel turns from "0000" to a target
// combination without ever passing through a deadend.
//
// The puzzle is a shortest-path query on LockGraph's wheel-turn graph, so the
// only thing this class contributes is the start combination and the choice of
// how to search.
internal static class OpenTheLockSolution
{
    // A wheel's digits, which turn 9 to 0 and 0 to 9.
    private const char FirstDigit = '0';
    private const char LastDigit = '9';

    // The textbook answer: BCL Queue + HashSet, generating each of the 8 candidate
    // turns on the fly and never materializing the graph. Deliberately written
    // without this repo's primitives - it turns the wheels itself rather than through
    // LockGraph, whose model a baseline may not read (ARCHITECTURE 17.5) - so it is the
    // arm the composed solution below has to justify itself against.
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
        foreach (var neighbor in OneTurnAway(combination))
        {
            if (!walk.Deadends.Has(neighbor) && walk.Visited.Add(neighbor))
            {
                walk.Queue.Enqueue((neighbor, turns + 1));
            }
        }
    }

    // The 8 combinations one turn away, dead or alive: each wheel one click up, then one
    // click down - the order LockGraph wires its edges in, so both arms search alike.
    private static IEnumerable<string> OneTurnAway(string combination)
    {
        var digits = combination.ToCharArray();

        for (var wheel = 0; wheel < digits.Length; wheel++)
        {
            var original = digits[wheel];

            digits[wheel] = ClickUp(original);
            yield return new string(digits);

            digits[wheel] = ClickDown(original);
            yield return new string(digits);

            digits[wheel] = original;
        }
    }

    private static char ClickUp(char digit)
    {
        if (digit == LastDigit)
        {
            return FirstDigit;
        }

        return (char)(digit + 1);
    }

    private static char ClickDown(char digit)
    {
        if (digit == FirstDigit)
        {
            return LastDigit;
        }

        return (char)(digit - 1);
    }

    private readonly record struct TurnWalk(
        Set<string> Deadends,
        HashSet<string> Visited,
        Queue<(string Combination, int Turns)> Queue);

    // This repo's own BFS: BreadthFirstDistances is already exactly "distance from
    // a root to every node", so the puzzle reduces to one lookup in the result -
    // the same composition WordLadder uses for LC 127.
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

        var distances = BreadthFirstDistances.From<LockNode, LockTopology, ListChildren<LockNode>>(startNode);

        return distances.TryGetValue(targetNode, out var distance) ? distance : LeetCodeAnswer.None;
    }
}
