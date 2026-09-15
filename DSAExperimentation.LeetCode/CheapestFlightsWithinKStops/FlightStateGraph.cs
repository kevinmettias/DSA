namespace DSAExperimentation.LeetCode.CheapestFlightsWithinKStops;

// LC 787's flight list expanded into the layered (city, edgesUsed) state space
// FlightState describes: one node per city per stop layer, and one edge per flight
// per layer, always from layer L to layer L+1. Building it is the whole trick -
// once it exists, an unmodified Dijkstra answers the K-stop question.
//
// A prepared instance is also what the solution's hoisted overload takes
// (ARCHITECTURE.md section 17.4), so a benchmark charges this construction to
// [GlobalSetup] rather than to the search it measures.
internal sealed class FlightStateGraph
{
    private readonly FlightState[,] _states;

    // The most edges any path in this graph can use: LeetCode's "at most K stops"
    // is "at most K+1 edges", and that is the last stop layer.
    public int MaxEdges => _states.GetLength(1) - 1;

    private FlightStateGraph(FlightState[,] states) => _states = states;

    public static FlightStateGraph Build(int n, int[][] flights, int k)
    {
        var maxEdges = k + 1;
        var states = BuildStates(n, maxEdges);

        AddFlightEdges(flights, states, maxEdges);

        return new FlightStateGraph(states);
    }

    private static FlightState[,] BuildStates(int n, int maxEdges)
    {
        var states = new FlightState[n, maxEdges + 1];

        for (var city = 0; city < n; city++)
        {
            for (var layer = 0; layer <= maxEdges; layer++)
            {
                states[city, layer] = new FlightState(city, layer);
            }
        }

        return states;
    }

    private static void AddFlightEdges(int[][] flights, FlightState[,] states, int maxEdges)
    {
        foreach (var flight in flights)
        {
            var (from, to, price) = (flight[0], flight[1], flight[2]);

            for (var layer = 0; layer < maxEdges; layer++)
            {
                states[from, layer].Edges.Add((price, states[to, layer + 1]));
            }
        }
    }

    public FlightState At(int city, int stops) => _states[city, stops];
}
