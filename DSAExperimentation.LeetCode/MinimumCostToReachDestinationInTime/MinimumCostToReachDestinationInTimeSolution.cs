using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

// LeetCode 1928. Minimum Cost to Reach Destination in Time: the cheapest sum of
// passing fees on any route from city 0 to city n-1 that takes at most maxTime
// minutes, or -1 when no route fits the budget. Fees are charged for every city
// entered, including the first and the last, and a city entered twice is paid for
// twice.
//
// Two costs are being traded off at once - minutes and money - and Dijkstra
// minimizes exactly one thing, so the budget is expressed as graph SHAPE rather
// than as algorithm logic: TimeCityGraph's (city, elapsedTime) expansion has one
// node per arrival time a city is actually reachable at, every edge weighted by
// the fee for entering its target, and ShortestPath.Dijkstra runs over it
// completely unmodified. Fees are positive, exactly Dijkstra's own precondition,
// and the answer is the cheapest of the destination's states plus the fee for the
// city the trip starts in, which no incoming edge ever charges.
//
// MinCostByNaiveDfs is the textbook alternative that expansion has to justify
// itself against: every walk the budget can pay for, unmemoized, so the same
// (city, elapsedTime) position is re-derived once per route that reaches it.
internal static class MinimumCostToReachDestinationInTimeSolution
{
    // Both strategies use int.MaxValue as their running "nothing found yet" cost,
    // and both map it onto LeetCode's -1 at the boundary rather than propagating
    // it. Fees never reach this, so it can never be a real answer.
    private const int NoRoute = int.MaxValue;

    // The textbook answer: walk out of city 0 and recurse into every road the
    // remaining budget can pay for, keeping the cheapest arrival at the
    // destination. Deliberately written without this repo's primitives - only the
    // adjacency container it is handed is a repo type (ARCHITECTURE.md section
    // 17.5). It terminates on the budget alone: every road costs at least one
    // minute (LeetCode's own constraint), so the recursion depth is bounded by
    // maxTime even though roads are two-way and the walk may revisit a city.
    public static int MinCostByNaiveDfs(int maxTime, int[][] edges, int[] passingFees)
    {
        var roads = RoadNetwork.Build(edges, passingFees);

        return MinCostByNaiveDfs(roads, maxTime);
    }

    public static int MinCostByNaiveDfs(RoadNetwork roads, int maxTime)
    {
        var best = BestFeeSum(roads, city: 0, remainingTime: maxTime);

        return best == NoRoute ? LeetCodeAnswer.None : best;
    }

    // Returns the cheapest fee total for finishing the trip from this city with
    // this much time left, INCLUDING the fee for the city itself - so the top-level
    // call already accounts for city 0.
    private static int BestFeeSum(RoadNetwork roads, int city, int remainingTime)
    {
        if (city == roads.Destination)
        {
            return roads.FeeAt(city);
        }

        var onward = CheapestOnwardTrip(roads, city, remainingTime);

        if (onward == NoRoute)
        {
            return NoRoute;
        }

        return roads.FeeAt(city) + onward;
    }

    // The cheapest way to finish the trip from one of this city's roads, or
    // NoRoute when none of them leads anywhere the remaining minutes can reach.
    private static int CheapestOnwardTrip(RoadNetwork roads, int city, int remainingTime)
    {
        var best = NoRoute;

        foreach (var (to, time) in roads.RoadsFrom(city))
        {
            if (time > remainingTime)
            {
                continue;
            }

            var throughRoad = BestFeeSum(roads, to, remainingTime - time);

            if (throughRoad < best)
            {
                best = throughRoad;
            }
        }

        return best;
    }

    // This repo's own Dijkstra over the time-expanded graph: one run from
    // (city 0, minute 0) settles every reachable state, and the answer is the
    // cheapest state sitting on the destination city, whatever minute it arrives.
    public static int MinCostByStateExpandedDijkstra(int maxTime, int[][] edges, int[] passingFees)
    {
        var graph = TimeCityGraph.Build(maxTime, edges, passingFees);

        return MinCostByStateExpandedDijkstra(graph);
    }

    public static int MinCostByStateExpandedDijkstra(TimeCityGraph graph)
    {
        var distances = ShortestPath.Dijkstra<
            TimeCityNode, TimeCityEdgeTopology, ListEdges<TimeCityNode, int>, int>(graph.Start);

        var best = CheapestArrival(distances, graph);

        if (best == NoRoute)
        {
            return LeetCodeAnswer.None;
        }

        return graph.StartFee + best;
    }

    private static int CheapestArrival(Dictionary<TimeCityNode, int> distances, TimeCityGraph graph)
    {
        var best = NoRoute;

        foreach (var state in graph.DestinationStates)
        {
            if (distances.TryGetValue(state, out var cost) && cost < best)
            {
                best = cost;
            }
        }

        return best;
    }
}
