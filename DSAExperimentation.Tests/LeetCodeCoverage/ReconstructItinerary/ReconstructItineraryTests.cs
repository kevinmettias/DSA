using DSAExperimentation.LeetCode.ReconstructItinerary;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReconstructItinerary;

// Harness only. Both selection strategies are ReconstructItinerarySolution's - this
// file just pins them to LeetCode's published examples, including the
// multiple-valid-routes case that requires lexicographic tie-breaking to reach the
// expected itinerary.
public sealed class ReconstructItineraryTests
{
    public static TheoryData<string[][], List<string>> Examples =>
        new()
        {
            {
                [["MUC", "LHR"], ["JFK", "MUC"], ["SFO", "SJC"], ["LHR", "SFO"]],
                ["JFK", "MUC", "LHR", "SFO", "SJC"]
            },
            {
                [["JFK", "SFO"], ["JFK", "ATL"], ["SFO", "ATL"], ["ATL", "JFK"], ["ATL", "SFO"]],
                ["JFK", "ATL", "JFK", "SFO", "ATL", "SFO"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindItineraryByLinearScan_LeetCodeExamples_ReturnsLexicallySmallestRoute(
        string[][] tickets, List<string> expected) =>
        Assert.Equal(expected, ReconstructItinerarySolution.FindItineraryByLinearScan(tickets));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindItineraryByHeapSelection_LeetCodeExamples_ReturnsLexicallySmallestRoute(
        string[][] tickets, List<string> expected) =>
        Assert.Equal(expected, ReconstructItinerarySolution.FindItineraryByHeapSelection(tickets));
}
