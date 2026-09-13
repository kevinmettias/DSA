using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.CheapestFlightsWithinKStops;

internal readonly struct FlightStateTopology : IEdgeTopology<FlightState, ListEdges<FlightState, int>, int>
{
    public static ListEdges<FlightState, int> GetEdges(FlightState node) => new(node.Edges);
}
