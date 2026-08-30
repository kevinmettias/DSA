namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a Race Car scenario (LC 818): the (position, speed) state space is
// otherwise unbounded (speed doubles every 'A'), so it's pre-materialized within a
// generous, target-scaled bound - |speed| never needs to exceed the smallest power
// of two past 4*target, and position stays within that same margin either side of
// zero - before either the naive BFS or Reduce.Graph ever runs over it.
internal static class RaceCarGraphs
{
    public static (
        Dictionary<(int Position, int Speed), RaceCarNode> NodesByState, List<int> Speeds, RaceCarNode Start)
        BuildGraph(int target)
    {
        var positionBound = 4 * target + 2;
        var maxSpeedMagnitude = 1;

        while (maxSpeedMagnitude < positionBound)
        {
            maxSpeedMagnitude *= 2;
        }

        var speeds = new List<int>();

        for (var magnitude = 1; magnitude <= maxSpeedMagnitude; magnitude *= 2)
        {
            speeds.Add(magnitude);
            speeds.Add(-magnitude);
        }

        var nodesByState = new Dictionary<(int, int), RaceCarNode>();

        for (var position = -positionBound; position <= positionBound; position++)
        {
            foreach (var speed in speeds)
            {
                nodesByState[(position, speed)] = new RaceCarNode(position, speed);
            }
        }

        foreach (var ((position, speed), node) in nodesByState)
        {
            if (nodesByState.TryGetValue((position + speed, speed * 2), out var accelerateNode))
            {
                node.Neighbors.Add(accelerateNode);
            }

            node.Neighbors.Add(nodesByState[(position, speed > 0 ? -1 : 1)]);
        }

        return (nodesByState, speeds, nodesByState[(0, 1)]);
    }
}
