namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToReachDestinationInTime.Fixtures;

// One node per reachable (city, elapsedTime) state in the time-expanded graph -
// city/time pairs are never revisited under a different identity (see
// MinimumCostToReachDestinationInTimeTests' GetOrCreate), so Edges only ever grows
// as new outgoing transitions are discovered during the bounded BFS expansion.
internal sealed class TimeCityNode(int city, int time)
{
    public int City { get; } = city;

    public int Time { get; } = time;

    public List<(int Fee, TimeCityNode Target)> Edges { get; } = [];

    public override string ToString() => $"(City={City}, Time={Time})";
}
