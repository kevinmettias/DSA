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
internal sealed class LockGraph
{
    private readonly HashMap<string, LockNode> _nodesByCombination;

    private LockGraph(HashMap<string, LockNode> nodesByCombination) =>
        _nodesByCombination = nodesByCombination;

    // Every non-dead combination becomes a node, wired to whichever of its 8
    // wheel-turn neighbors also survived.
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

    public bool TryGetNode(string combination, out LockNode node) =>
        _nodesByCombination.TryGetValue(combination, out node);

    // The 8 combinations one wheel-turn away, dead or alive - pure wheel
    // arithmetic, so callers that never materialize the graph can use it too.
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
