using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.RaceCar.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RaceCar;

// LeetCode 818. Race Car: the car's (position, speed) pairs form an implicit
// unweighted graph - one edge per 'A' (accelerate: position += speed, speed *= 2)
// or 'R' (reverse: speed flips to +-1, position unchanged) command - so the fewest
// commands to reach target is exactly Reduce.Graph's own BFS distance, the same
// composition OpenTheLock already uses for LC 752. The (position, speed) state
// space is otherwise unbounded (speed doubles every 'A'), so it's pre-materialized
// within a generous, target-scaled bound before Reduce.Graph ever runs - the same
// "pre-materialize a bounded implicit graph" shape CheapestFlights/JumpGameII/
// OpenTheLock all already use for a problem with no natural finite node set.
public sealed partial class RaceCarTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 4)]
    [InlineData(3, 2)]
    [InlineData(6, 5)]
    public void MinCommands_ClassicExamples_ReturnsShortestCommandCount(int target, int expected)
        => Assert.Equal(expected, MinCommands(target));

    private static int MinCommands(int target)
    {
        var (nodesByState, speeds) = BuildStateGraph(target);
        var start = nodesByState[(0, 1)];

        var distances = Reduce.Graph<
            RaceCarNode, RaceCarTopology, ListChildren<RaceCarNode>,
            NaturalChildOrder<RaceCarNode, ListChildren<RaceCarNode>>, ListChildren<RaceCarNode>,
            BreadthFirstReduceOrder<RaceCarNode>,
            DistanceMapReduceAlgebra<RaceCarNode>, Dictionary<RaceCarNode, int>>(start);

        var best = int.MaxValue;

        foreach (var speed in speeds)
        {
            if (nodesByState.TryGetValue((target, speed), out var node) &&
                distances.TryGetValue(node, out var distance) && distance < best)
            {
                best = distance;
            }
        }

        return best;
    }

    // Bounds the otherwise-unbounded (position, speed) state space: |speed| never
    // needs to exceed the smallest power of two past 4*target (ample headroom to
    // overshoot and correct with an 'R'), and position stays within that same
    // margin either side of zero.
    private static (Dictionary<(int Position, int Speed), RaceCarNode> NodesByState, List<int> Speeds) BuildStateGraph(
        int target)
    {
        var positionBound = 4 * target + 2;
        var maxSpeedMagnitude = ComputeMaxSpeedMagnitude(positionBound);
        var speeds = BuildSpeeds(maxSpeedMagnitude);
        var nodesByState = BuildNodes(positionBound, speeds);
        WireEdges(nodesByState);

        return (nodesByState, speeds);
    }

    private static int ComputeMaxSpeedMagnitude(int positionBound)
    {
        var maxSpeedMagnitude = 1;

        while (maxSpeedMagnitude < positionBound)
        {
            maxSpeedMagnitude *= 2;
        }

        return maxSpeedMagnitude;
    }

    private static List<int> BuildSpeeds(int maxSpeedMagnitude)
    {
        var speeds = new List<int>();

        for (var magnitude = 1; magnitude <= maxSpeedMagnitude; magnitude *= 2)
        {
            speeds.Add(magnitude);
            speeds.Add(-magnitude);
        }

        return speeds;
    }

    private static Dictionary<(int Position, int Speed), RaceCarNode> BuildNodes(int positionBound, List<int> speeds)
    {
        var nodesByState = new Dictionary<(int, int), RaceCarNode>();

        for (var position = -positionBound; position <= positionBound; position++)
        {
            foreach (var speed in speeds)
            {
                nodesByState[(position, speed)] = new RaceCarNode(position, speed);
            }
        }

        return nodesByState;
    }

    private static void WireEdges(Dictionary<(int Position, int Speed), RaceCarNode> nodesByState)
    {
        foreach (var ((position, speed), node) in nodesByState)
        {
            if (nodesByState.TryGetValue((position + speed, speed * 2), out var accelerateNode))
            {
                node.Neighbors.Add(accelerateNode);
            }

            node.Neighbors.Add(nodesByState[(position, speed > 0 ? -1 : 1)]);
        }
    }
}
