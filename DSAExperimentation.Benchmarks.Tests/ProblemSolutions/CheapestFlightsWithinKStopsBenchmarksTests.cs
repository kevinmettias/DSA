using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheapestFlightsWithinKStopsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the unmemoized DFS over every itinerary within the
// stop bound against Dijkstra run over the (city, edgesUsed) state expansion of the same flights -
// so a harness whose arms disagree is timing two different problems. Both arms answer with a single
// int, the cheapest fare or the problem's "no itinerary inside the stop bound" sentinel, so
// agreement between them says the two searches settled on the same fare for the same network.
public sealed partial class CheapestFlightsWithinKStopsBenchmarksTests
{
    private const int SmallestCityCount = 15;

    [Fact]
    public void Setup_SameCityCount_RebuildsTheSameFlightNetwork()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The flight list and the whole (city, edgesUsed) expansion are private, and both arms
        // reduce the network to one fare, so two harnesses built from the same CityCount reporting
        // the same fare for each search is the reading a rebuild can be pinned to: one seeded
        // draw means the same flights, and the same flights mean the same two fares.
        Assert.Equal(first.NaiveDfs(), second.NaiveDfs());
        Assert.Equal(first.DijkstraOverStopLayeredGraph(), second.DijkstraOverStopLayeredGraph());
    }

    [Fact]
    public void NaiveDfs_FifteenCitySeededNetwork_AgreesWithDijkstraOverStopLayeredGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DijkstraOverStopLayeredGraph(), harness.NaiveDfs());
    }

    [Fact]
    public void DijkstraOverStopLayeredGraph_FifteenCitySeededNetwork_AgreesWithNaiveDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveDfs(), harness.DijkstraOverStopLayeredGraph());
    }

    private static CheapestFlightsWithinKStopsBenchmarks BuildHarness()
    {
        var harness = new CheapestFlightsWithinKStopsBenchmarks { CityCount = SmallestCityCount };
        harness.Setup();

        return harness;
    }
}
