namespace DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

// LC 1928's road map expanded into the (city, elapsedTime) state space TimeCityNode
// describes: one node per arrival time a city can actually be reached at within the
// budget, and one edge (u,t) -> (v,t+w) per road, weighted by the fee for entering
// v. Building it is the whole trick - once it exists, an unmodified Dijkstra answers
// a question ShortestPath has no time-budget parameter for.
//
// Only states the bounded walk actually reaches are materialized, so the node count
// is bounded by cities x budget rather than by anything exponential. Road times are
// at least one minute, so time strictly increases along every edge and the walk
// terminates on the budget alone.
//
// A prepared instance is also what the solution's hoisted overload takes
// (ARCHITECTURE.md section 17.4), so a benchmark charges this construction to
// [GlobalSetup] rather than to the search it measures.
internal sealed class TimeCityGraph(TimeCityNode start, List<TimeCityNode> destinationStates, int startFee)
{
    // The trip always begins in city 0 at time 0.
    public TimeCityNode Start { get; } = start;

    // Every state that sits on the destination city, at any arrival time within
    // the budget - the trip may finish at whichever of them is cheapest.
    public List<TimeCityNode> DestinationStates { get; } = destinationStates;

    // Dijkstra starts the source at distance zero, so no edge ever charges the fee
    // for the city the trip begins in; it is added back once, on top of the best
    // distance found.
    public int StartFee { get; } = startFee;

    public static TimeCityGraph Build(int maxTime, int[][] edges, int[] passingFees)
    {
        var roads = RoadNetwork.Build(edges, passingFees);

        return Build(maxTime, roads);
    }

    public static TimeCityGraph Build(int maxTime, RoadNetwork roads)
    {
        var frontier = new Queue<TimeCityNode>();
        var expansion = new Expansion(maxTime, roads, [], [], frontier);
        var origin = StateAt(expansion, city: 0, time: 0);

        Expand(expansion, origin);

        return new TimeCityGraph(origin, expansion.DestinationStates, roads.FeeAt(0));
    }

    private static void Expand(Expansion expansion, TimeCityNode origin)
    {
        expansion.Frontier.Enqueue(origin);

        while (expansion.Frontier.Count > 0)
        {
            var current = expansion.Frontier.Dequeue();

            foreach (var (to, time) in expansion.Roads.RoadsFrom(current.City))
            {
                Connect(expansion, current, to, current.Time + time);
            }
        }
    }

    private static void Connect(Expansion expansion, TimeCityNode current, int city, int time)
    {
        if (time > expansion.MaxTime)
        {
            return;
        }

        var isNew = !expansion.States.ContainsKey((city, time));
        var next = StateAt(expansion, city, time);

        current.Edges.Add((expansion.Roads.FeeAt(city), next));

        if (isNew)
        {
            expansion.Frontier.Enqueue(next);
        }
    }

    private static TimeCityNode StateAt(Expansion expansion, int city, int time)
    {
        var key = (city, time);

        if (expansion.States.TryGetValue(key, out var state))
        {
            return state;
        }

        state = new TimeCityNode(city, time);
        expansion.States[key] = state;

        if (city == expansion.Roads.Destination)
        {
            expansion.DestinationStates.Add(state);
        }

        return state;
    }

    // The parts of the expansion that never change while it runs, carried as one
    // value the way CheapestFlightsWithinKStops' own FlightSearch is.
    private readonly record struct Expansion(
        int MaxTime,
        RoadNetwork Roads,
        Dictionary<(int City, int Time), TimeCityNode> States,
        List<TimeCityNode> DestinationStates,
        Queue<TimeCityNode> Frontier);
}
