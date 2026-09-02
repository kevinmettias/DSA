namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a Race Car scenario (LC 818): the (position, speed) state space is
// otherwise unbounded (speed doubles every 'A'), so it's pre-materialized within a
// generous, target-scaled bound - |speed| never needs to exceed the smallest power
// of two past 4*target, and position stays within that same margin either side of
// zero - before either the naive BFS or Reduce.Graph ever runs over it.
internal static class RaceCarGraphs
{
    private const int PositionBoundMultiplier = 4;
    private const int PositionBoundPadding = 2;
    private const int SpeedDoublingFactor = 2;

    public static (
        Dictionary<(int Position, int Speed), RaceCarNode> NodesByState, List<int> Speeds, RaceCarNode Start)
        BuildGraph(int target)
    {
        var positionBound = (PositionBoundMultiplier * target) + PositionBoundPadding;
        var speeds = BuildSpeedMagnitudes(positionBound);
        var nodesByState = BuildNodes(positionBound, speeds);
        WireNeighbors(nodesByState);

        return (nodesByState, speeds, nodesByState[(0, 1)]);
    }

    private static List<int> BuildSpeedMagnitudes(int positionBound)
    {
        var maxSpeedMagnitude = 1;

        while (maxSpeedMagnitude < positionBound)
        {
            maxSpeedMagnitude *= SpeedDoublingFactor;
        }

        var speeds = new List<int>();

        for (var magnitude = 1; magnitude <= maxSpeedMagnitude; magnitude *= SpeedDoublingFactor)
        {
            speeds.Add(magnitude);
            speeds.Add(-magnitude);
        }

        return speeds;
    }

    private static Dictionary<(int, int), RaceCarNode> BuildNodes(int positionBound, List<int> speeds)
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

    private static void WireNeighbors(Dictionary<(int, int), RaceCarNode> nodesByState)
    {
        foreach (var ((position, speed), node) in nodesByState)
        {
            if (nodesByState.TryGetValue((position + speed, speed * SpeedDoublingFactor), out var accelerateNode))
            {
                node.Neighbors.Add(accelerateNode);
            }

            node.Neighbors.Add(nodesByState[(position, speed > 0 ? -1 : 1)]);
        }
    }
}
