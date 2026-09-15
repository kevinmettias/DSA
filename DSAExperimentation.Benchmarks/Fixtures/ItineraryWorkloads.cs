using DSAExperimentation.LeetCode.ReconstructItinerary;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 332 - a synthetic ticket graph large enough to
// separate LinearScanSelection's O(k) scan-and-remove from HeapSelection's O(log k)
// repo Heap push/pop. Airport codes are single letters except index 0, which is
// pinned to StartAirport.Code ("JFK") so the synthetic graph is actually reachable
// from LeetCode's fixed starting airport.
internal static class ItineraryWorkloads
{
    private const int AirportCount = 26;

    public static string[][] BuildTickets(int ticketCount, int seed)
    {
        var random = new Random(seed);
        var airports = new string[AirportCount];
        airports[0] = StartAirport.Code;

        for (var i = 1; i < AirportCount; i++)
        {
            airports[i] = ((char)('A' + i)).ToString();
        }

        var tickets = new string[ticketCount][];

        for (var i = 0; i < ticketCount; i++)
        {
            tickets[i] = [airports[random.Next(airports.Length)], airports[random.Next(airports.Length)]];
        }

        return tickets;
    }
}
