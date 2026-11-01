using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Domain.Locks;

// The Cayley graph of a 4-wheel rotary combination lock over Z10^4: 10,000
// combinations, each adjacent to the 8 reachable by turning one wheel one click in
// either direction, minus any combinations declared dead.
//
// This is the domain model, not an answer to any one puzzle about it - it knows
// how a lock's wheels turn and which combinations are reachable, and nothing about
// what a caller wants to find out. Callers supply their own start and target.
/// <summary>
/// The combinations of a four-wheel rotary lock that are not dead, each joined to the live ones one
/// wheel-turn away.
/// </summary>
public sealed class LockGraph
{
    private readonly HashMap<string, LockNode> _nodesByCombination;

    private LockGraph(HashMap<string, LockNode> nodesByCombination) =>
        _nodesByCombination = nodesByCombination;

    // Every non-dead combination becomes a node, wired to whichever of its 8
    // wheel-turn neighbors also survived.
    /// <summary>Builds the graph of every combination except the dead ones.</summary>
    /// <param name="deadends">The combinations the lock refuses; none of them becomes a node.</param>
    /// <returns>The graph, with each node joined to its live wheel-turn neighbors.</returns>
    public static LockGraph Build(IEnumerable<string> deadends)
    {
        var dead = new Set<string>(deadends);
        var nodesByCombination = BuildNodes(dead);

        WireEdges(nodesByCombination);

        return new LockGraph(nodesByCombination);
    }

    private static HashMap<string, LockNode> BuildNodes(Set<string> deadends)
    {
        var nodesByCombination = new HashMap<string, LockNode>();

        for (var i = 0; i < LockWheels.CombinationSpace; i++)
        {
            var combination = LockWheels.Combination(i);

            if (!deadends.Has(combination))
            {
                nodesByCombination.Set(combination, new LockNode(combination));
            }
        }

        return nodesByCombination;
    }

    private static void WireEdges(HashMap<string, LockNode> nodesByCombination)
    {
        foreach (var node in nodesByCombination.Values)
        {
            foreach (var neighborCombination in WheelTurnNeighbors(node.Combination))
            {
                if (nodesByCombination.TryGetValue(neighborCombination, out var neighborNode))
                {
                    node.Neighbors.Add(neighborNode);
                }
            }
        }
    }

    /// <summary>Looks up the node for a combination.</summary>
    /// <param name="combination">A four-digit combination such as <c>"0000"</c>.</param>
    /// <param name="node">The combination's node when it is live; otherwise unspecified.</param>
    /// <returns>True when the combination is live; false when it is dead or not a combination.</returns>
    public bool TryGetNode(string combination, out LockNode node) =>
        _nodesByCombination.TryGetValue(combination, out node);

    // The 8 combinations one wheel-turn away, dead or alive - the pure wheel
    // arithmetic WireEdges joins nodes by.
    /// <summary>Lists the eight combinations one wheel-turn away, dead or alive.</summary>
    /// <param name="combination">A four-digit combination.</param>
    /// <returns>Each wheel turned one click up and then one click down, wheel by wheel.</returns>
    public static IEnumerable<string> WheelTurnNeighbors(string combination)
    {
        for (var wheel = 0; wheel < LockWheels.Count; wheel++)
        {
            var digit = combination[wheel] - '0';
            yield return Turn(combination, wheel, (digit + 1) % LockWheels.Modulus);
            yield return Turn(combination, wheel, (digit + LockWheels.Modulus - 1) % LockWheels.Modulus);
        }
    }

    private static string Turn(string combination, int wheel, int newDigit)
    {
        var chars = combination.ToCharArray();
        chars[wheel] = (char)('0' + newDigit);
        return new string(chars);
    }
}
