using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.Tests.LeetCodeCoverage.OpenTheLock.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OpenTheLock;

// LeetCode 752. Open the Lock: combinations are nodes of an implicit graph, with an
// edge between any two combinations exactly one wheel-turn apart (each of 4 wheels,
// +1 or -1 mod 10, deadends excluded entirely). The fewest turns to reach target is
// exactly Reduce.Graph's own BFS distance from "0000" - DistanceMapReduceAlgebra,
// the same "distance to some specific target" composition WordLadderTests already
// uses for LC 127. Unlike WordLadder's pairwise one-letter-apart scan, edges here
// are cheap to generate directly from wheel arithmetic (8 candidates per node)
// instead of comparing every pair.
public sealed partial class OpenTheLockTests
{
    [Fact]
    public void MinTurns_ClassicExample_ReturnsShortestUnblockedTurnCount()
    {
        var turns = OpenLock(["0201", "0101", "0102", "1212", "2002"], "0202");

        Assert.Equal(6, turns);
    }

    [Fact]
    public void MinTurns_TargetSurroundedByDeadends_ReturnsMinusOne()
    {
        var turns = OpenLock(
            ["8887", "8889", "8878", "8898", "8788", "8988", "7888", "9888"], "8888");

        Assert.Equal(-1, turns);
    }

    private static int OpenLock(List<string> deadends, string target)
    {
        var deadendSet = new HashSet<string>(deadends);
        var nodesByCombo = BuildGraph(deadendSet);

        if (!nodesByCombo.TryGetValue("0000", out var startNode) ||
            !nodesByCombo.TryGetValue(target, out var targetNode))
        {
            return -1;
        }

        var distances = Reduce.Graph<
            LockNode, LockTopology, ListChildren<LockNode>,
            NaturalChildOrder<LockNode, ListChildren<LockNode>>, ListChildren<LockNode>,
            BreadthFirstReduceOrder<LockNode>,
            DistanceMapReduceAlgebra<LockNode>, Dictionary<LockNode, int>>(startNode);

        return distances.TryGetValue(targetNode, out var distance) ? distance : -1;
    }

    // Every non-deadend 4-digit combination becomes a node.
    private static HashMap<string, LockNode> BuildGraph(HashSet<string> deadends)
    {
        var nodesByCombo = new HashMap<string, LockNode>();

        for (var i = 0; i < 10_000; i++)
        {
            var combo = i.ToString("D4");

            if (!deadends.Contains(combo))
            {
                nodesByCombo.Set(combo, new LockNode(combo));
            }
        }

        foreach (var node in nodesByCombo.Values)
        {
            foreach (var neighborCombo in WheelTurnNeighbors(node.Combination))
            {
                if (nodesByCombo.TryGetValue(neighborCombo, out var neighborNode))
                {
                    node.Neighbors.Add(neighborNode);
                }
            }
        }

        return nodesByCombo;
    }

    private static IEnumerable<string> WheelTurnNeighbors(string combo)
    {
        for (var wheel = 0; wheel < 4; wheel++)
        {
            var digit = combo[wheel] - '0';
            yield return Turn(combo, wheel, (digit + 1) % 10);
            yield return Turn(combo, wheel, (digit + 9) % 10);
        }
    }

    private static string Turn(string combo, int wheel, int newDigit)
    {
        var chars = combo.ToCharArray();
        chars[wheel] = (char)('0' + newDigit);
        return new string(chars);
    }
}
