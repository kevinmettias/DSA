using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ItineraryWorkloads (ARCHITECTURE 17.7). The reading depends on LC 332's ticket
// list being well-formed rows over one fixed set of three-letter airport codes, that set containing the
// fixed starting airport the problem's own reconstruction begins from, and on the guarantee LC 332
// states: the tickets form at least one itinerary from that airport using each of them once, and no
// ticket flies from an airport to itself.
public sealed partial class ItineraryWorkloadsTests
{
    private const int TicketCount = 64;
    private const int Seed = 332; // LC problem number
    private const int TicketFieldCount = 2; // FromAirport, ToAirport
    private const string StartAirportCode = "JFK"; // LC 332's own fixed start
    private const char FirstGeneratedCode = 'B';
    private const char LastGeneratedCode = 'Z';
    private const int CodeLength = 3;

    // An open walk leaves its start one ticket more often than it returns, and ends one ticket short
    // somewhere else; every other airport it passes is balanced.
    private const int OpenWalkUnbalancedAirports = 2;
    private const int StartSurplus = 1;
    private const int EndSurplus = -1;

    [Fact]
    public void BuildTickets_TicketCount_ReturnsOneFromToPairPerTicket()
    {
        var tickets = ItineraryWorkloads.BuildTickets(TicketCount, Seed);

        Assert.Equal(TicketCount, tickets.Length);
        Assert.All(tickets, ticket => Assert.Equal(TicketFieldCount, ticket.Length));
    }

    [Fact]
    public void BuildTickets_EveryAirport_NamesACodeOfTheSyntheticNetwork()
    {
        var tickets = ItineraryWorkloads.BuildTickets(TicketCount, Seed);
        var codes = AirportCodes();

        Assert.Contains(StartAirportCode, codes);
        Assert.All(tickets, ticket => Assert.All(ticket, airport => Assert.Contains(airport, codes)));
    }

    [Fact]
    public void BuildTickets_EveryTicket_FliesToADifferentAirport() =>
        Assert.All(
            ItineraryWorkloads.BuildTickets(TicketCount, Seed),
            ticket => Assert.NotEqual(ticket[0], ticket[1]));

    // The itinerary LC 332 guarantees, read off the tickets alone: each airport's departures and
    // arrivals balance as a walk from the start's do, and every ticket leaves an airport the start
    // reaches.
    [Fact]
    public void BuildTickets_Tickets_ChainIntoOneItineraryFromTheStartAirport()
    {
        var tickets = ItineraryWorkloads.BuildTickets(TicketCount, Seed);
        var unbalanced = OutboundSurplus(tickets)
            .Where(entry => entry.Value != 0)
            .ToDictionary(entry => entry.Key, entry => entry.Value);
        var reachable = ReachableFromStart(tickets);

        Assert.True(unbalanced.Count == 0 || IsOpenWalkFromStart(unbalanced));
        Assert.All(tickets, ticket => Assert.Contains(ticket[0], reachable));
    }

    [Fact]
    public void BuildTickets_SameSeed_ReturnsTheSameTickets() =>
        Assert.Equal(
            AnswerGraphText.Of(ItineraryWorkloads.BuildTickets(TicketCount, Seed)),
            AnswerGraphText.Of(ItineraryWorkloads.BuildTickets(TicketCount, Seed)));

    private static HashSet<string> AirportCodes() =>
        [StartAirportCode, .. Enumerable.Range(FirstGeneratedCode, LastGeneratedCode - FirstGeneratedCode + 1)
            .Select(code => new string((char)code, CodeLength))];

    // Departures minus arrivals, per airport.
    private static Dictionary<string, int> OutboundSurplus(string[][] tickets)
    {
        var surplus = new Dictionary<string, int>();

        foreach (var ticket in tickets)
        {
            surplus[ticket[0]] = surplus.GetValueOrDefault(ticket[0]) + 1;
            surplus[ticket[1]] = surplus.GetValueOrDefault(ticket[1]) - 1;
        }

        return surplus;
    }

    private static bool IsOpenWalkFromStart(Dictionary<string, int> unbalanced) =>
        unbalanced.Count == OpenWalkUnbalancedAirports
        && unbalanced.GetValueOrDefault(StartAirportCode) == StartSurplus
        && unbalanced.ContainsValue(EndSurplus);

    private static HashSet<string> ReachableFromStart(string[][] tickets)
    {
        var reached = new HashSet<string> { StartAirportCode };
        var pending = new Queue<string>(reached);

        while (pending.Count > 0)
        {
            var airport = pending.Dequeue();

            foreach (var ticket in tickets)
            {
                if (ticket[0] == airport && reached.Add(ticket[1]))
                {
                    pending.Enqueue(ticket[1]);
                }
            }
        }

        return reached;
    }
}
