using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ItineraryWorkloads (ARCHITECTURE 17.7). The reading depends on LC 332's ticket
// list being well-formed rows over one fixed set of airport codes, and that set containing the fixed
// starting airport the problem's own reconstruction begins from.
public sealed partial class ItineraryWorkloadsTests
{
    private const int TicketCount = 64;
    private const int Seed = 332; // LC problem number
    private const int TicketFieldCount = 2; // FromAirport, ToAirport
    private const string StartAirportCode = "JFK"; // LC 332's own fixed start
    private const char FirstGeneratedCode = 'B';
    private const char LastGeneratedCode = 'Z';

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
    public void BuildTickets_SameSeed_ReturnsTheSameTickets() =>
        Assert.Equal(
            AnswerText.Of(ItineraryWorkloads.BuildTickets(TicketCount, Seed)),
            AnswerText.Of(ItineraryWorkloads.BuildTickets(TicketCount, Seed)));

    private static HashSet<string> AirportCodes() =>
        [StartAirportCode, .. Enumerable.Range(FirstGeneratedCode, LastGeneratedCode - FirstGeneratedCode + 1)
            .Select(code => ((char)code).ToString())];
}
