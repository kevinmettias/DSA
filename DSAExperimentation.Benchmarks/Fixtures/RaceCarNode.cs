namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' RaceCar RaceCarNode fixture: one node per
// (position, speed) state, Neighbors holding the (at most two) states reachable by
// a single 'A' (accelerate) or 'R' (reverse) command.
internal sealed class RaceCarNode(int position, int speed)
{
    public int Position { get; } = position;

    public int Speed { get; } = speed;

    public List<RaceCarNode> Neighbors { get; } = [];
}
