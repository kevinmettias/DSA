namespace DSAExperimentation.LeetCode.ReconstructItinerary;

// LeetCode 332's fixed departure point: every itinerary starts at "JFK", whatever the
// ticket list says, and Hierholzer's walk begins there in both strategies. Separate
// from the solution because a benchmark generating a synthetic ticket graph has to
// pin an airport to the same code for the graph to be reachable from it at all.
internal static class StartAirport
{
    public const string Code = "JFK";
}
