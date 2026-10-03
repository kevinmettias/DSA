using DSAExperimentation.LeetCode.ReconstructItinerary;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 332 - a synthetic ticket graph large enough to
// separate LinearScanSelection's O(k) scan-and-remove from HeapSelection's O(log k)
// repo Heap push/pop. The tickets are the legs of a seeded random walk from
// StartAirport.Code ("JFK"), shuffled: the walk itself is an itinerary that uses every
// ticket once, which LC 332 guarantees exists, and every leg moves to a different
// airport, as its fromi != toi requires. Airport codes are three uppercase letters, as
// LC 332's are - "JFK" at index 0 and one letter tripled ("BBB" to "ZZZ") for the rest.
internal static class ItineraryWorkloads
{
    private const int AirportCount = 26;
    private const int CodeLength = 3;

    public static string[][] BuildTickets(int ticketCount, int seed)
    {
        var random = new Random(seed);
        var airports = AirportCodes();
        var legs = new string[ticketCount][];
        var current = 0;

        for (var i = 0; i < ticketCount; i++)
        {
            var next = NextAirport(current, random);
            legs[i] = [airports[current], airports[next]];
            current = next;
        }

        var order = SeededSequences.ShuffledZeroTo(ticketCount, random);

        return [.. order.Select(leg => legs[leg])];
    }

    private static string[] AirportCodes()
    {
        var airports = new string[AirportCount];
        airports[0] = StartAirport.Code;

        for (var i = 1; i < AirportCount; i++)
        {
            airports[i] = new string((char)('A' + i), CodeLength);
        }

        return airports;
    }

    // A uniformly random airport other than the current one: one draw over the other
    // AirportCount - 1 indices, stepping past the current index.
    private static int NextAirport(int current, Random random)
    {
        var draw = random.Next(AirportCount - 1);

        if (draw >= current)
        {
            return draw + 1;
        }

        return draw;
    }
}
