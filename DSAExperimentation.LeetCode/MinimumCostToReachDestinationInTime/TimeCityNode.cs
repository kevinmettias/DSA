namespace DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

// One node per reachable (city, elapsedTime) state in the time-expanded graph.
// LeetCode's road times are always at least one minute, so an edge always moves a
// state strictly forward in time and the expansion is acyclic by construction -
// which is what lets TimeCityGraph's bounded walk terminate without a visited
// guard beyond "already materialized this (city, time) pair." A pair is never
// created twice (see TimeCityGraph.StateAt), so Edges only grows as new outgoing
// transitions are discovered.
//
// This shape answers LC 1928 and nothing else, so it lives beside the solution
// rather than in Domain/ (ARCHITECTURE.md section 17.3).
internal sealed class TimeCityNode(int city, int time)
{
    public int City { get; } = city;

    public int Time { get; } = time;

    public List<(int Fee, TimeCityNode Target)> Edges { get; } = [];

    public override string ToString() => $"(City={City}, Time={Time})";
}
