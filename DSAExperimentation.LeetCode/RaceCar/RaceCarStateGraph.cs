namespace DSAExperimentation.LeetCode.RaceCar;

// LC 818's command graph, materialized over RaceCarStateSpace's bounded box: one
// RaceCarNode per (position, speed) state, and one edge per command - 'A' to
// (position + speed, speed * 2) when that state is still in the box, 'R' to
// (position, +-1), which always is.
//
// Building it is what turns an implicit, unbounded state space into something an
// unmodified graph BFS can run over. A prepared instance is also what the
// solution's hoisted overload takes (ARCHITECTURE.md section 17.4), so a benchmark
// charges this construction to [GlobalSetup] rather than to the search it measures.
internal sealed class RaceCarStateGraph
{
    private readonly Dictionary<(int Position, int Speed), RaceCarNode> _nodesByState;
    private readonly List<int> _speeds;

    private RaceCarStateGraph(Dictionary<(int Position, int Speed), RaceCarNode> nodesByState, List<int> speeds)
    {
        _nodesByState = nodesByState;
        _speeds = speeds;
    }

    // The state every run begins in: at the origin, moving forward at speed 1.
    public RaceCarNode Start => _nodesByState[(RaceCarStateSpace.StartPosition, RaceCarStateSpace.StartSpeed)];

    // Every speed a node in this graph can carry, so a caller asking "what is the
    // distance to this position" can scan the states that share it.
    public IReadOnlyList<int> Speeds => _speeds;

    public static RaceCarStateGraph Build(int target)
    {
        var space = RaceCarStateSpace.For(target);
        var speeds = space.Speeds();
        var nodesByState = BuildNodes(space, speeds);

        WireCommands(nodesByState);

        return new RaceCarStateGraph(nodesByState, speeds);
    }

    public bool TryGetNode(int position, int speed, out RaceCarNode node) =>
        _nodesByState.TryGetValue((position, speed), out node!);

    private static Dictionary<(int Position, int Speed), RaceCarNode> BuildNodes(
        RaceCarStateSpace space, List<int> speeds)
    {
        var nodesByState = new Dictionary<(int Position, int Speed), RaceCarNode>();

        for (var position = -space.PositionBound; position <= space.PositionBound; position++)
        {
            foreach (var speed in speeds)
            {
                nodesByState[(position, speed)] = new RaceCarNode(position, speed);
            }
        }

        return nodesByState;
    }

    private static void WireCommands(Dictionary<(int Position, int Speed), RaceCarNode> nodesByState)
    {
        foreach (var ((position, speed), node) in nodesByState)
        {
            var acceleratedSpeed = speed * RaceCarStateSpace.SpeedDoublingFactor;

            if (nodesByState.TryGetValue((position + speed, acceleratedSpeed), out var accelerateNode))
            {
                node.Neighbors.Add(accelerateNode);
            }

            node.Neighbors.Add(nodesByState[(position, ReversedSpeed(speed))]);
        }
    }

    // 'R' does not move the car: it drops the speed to a single step in the
    // opposite direction.
    private static int ReversedSpeed(int speed) =>
        speed > 0 ? -RaceCarStateSpace.StartSpeed : RaceCarStateSpace.StartSpeed;
}
