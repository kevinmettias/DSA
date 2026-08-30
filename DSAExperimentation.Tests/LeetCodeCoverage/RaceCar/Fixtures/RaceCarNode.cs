namespace DSAExperimentation.Tests.LeetCodeCoverage.RaceCar.Fixtures;

// One node per (position, speed) state the car can be in; Neighbors holds the (at
// most two) states reachable by a single 'A' (accelerate) or 'R' (reverse) command.
internal sealed class RaceCarNode(int position, int speed)
{
    public int Position { get; } = position;

    public int Speed { get; } = speed;

    public List<RaceCarNode> Neighbors { get; } = [];

    public override string ToString() => $"{Position}@{Speed}";
}
