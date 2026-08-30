using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheapestFlightsWithinKStops.Fixtures;

internal readonly struct FlightStateTopology : IEdgeTopology<FlightState, ListEdges<FlightState, int>, int>
{
    public static ListEdges<FlightState, int> GetEdges(FlightState node) => new(node.Edges);
}
