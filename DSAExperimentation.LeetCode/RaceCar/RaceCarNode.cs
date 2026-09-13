namespace DSAExperimentation.LeetCode.RaceCar;

// One node per (position, speed) state the car can be in; Neighbors holds the (at
// most two) states reachable by a single 'A' (accelerate: position += speed, speed
// *= 2) or 'R' (reverse: speed flips to +-1, position unchanged) command.
//
// LC 818's state semantics are the whole content of this type, so it stays beside
// the solution rather than in DataStructures/ (ARCHITECTURE.md section 17.3).
internal sealed class RaceCarNode(int position, int speed)
{
    public int Position { get; } = position;

    public int Speed { get; } = speed;

    public List<RaceCarNode> Neighbors { get; } = [];

    public override string ToString() => $"{Position}@{Speed}";
}
