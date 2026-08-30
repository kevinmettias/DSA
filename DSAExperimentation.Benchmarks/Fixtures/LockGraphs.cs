namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds an Open the Lock scenario (LC 752): every non-deadend 4-digit combination
// in the full 10,000-node Cayley graph over Z10^4 (8 wheel-turn edges per node), with
// a random scattering of deadends that almost never disconnects it - each node
// still has 8 candidate edges, so removing up to a few hundred of 10,000 nodes at
// random leaves the graph connected with overwhelming probability, the same
// "genuinely reachable, real BFS work" guarantee WordLadderGraphs.BuildChain gives
// by construction instead.
internal static class LockGraphs
{
    public const string Start = "0000";

    // Farthest possible combination from "0000": each wheel needs 5 turns (the
    // max over +1/-1 mod 10 arithmetic), forcing the full 20-turn BFS radius.
    public const string FarthestTarget = "5555";

    public static HashSet<string> BuildDeadends(int count, int seed)
    {
        var random = new Random(seed);
        var deadends = new HashSet<string>();

        while (deadends.Count < count)
        {
            var candidate = random.Next(10_000).ToString("D4");

            if (candidate != Start && candidate != FarthestTarget)
            {
                deadends.Add(candidate);
            }
        }

        return deadends;
    }

    public static (Dictionary<string, LockNode> NodesByCombo, LockNode StartNode) BuildGraph(HashSet<string> deadends)
    {
        var nodesByCombo = new Dictionary<string, LockNode>();

        for (var i = 0; i < 10_000; i++)
        {
            var combo = i.ToString("D4");

            if (!deadends.Contains(combo))
            {
                nodesByCombo[combo] = new LockNode(combo);
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

        return (nodesByCombo, nodesByCombo[Start]);
    }

    public static IEnumerable<string> WheelTurnNeighbors(string combo)
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
