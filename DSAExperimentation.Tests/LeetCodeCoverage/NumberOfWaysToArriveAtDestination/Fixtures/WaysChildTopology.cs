using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToArriveAtDestination.Fixtures;

// A neighbor only counts as a "next hop toward the destination" when it lies
// exactly on a shortest path - Dist[node] - weight == Dist[target], not merely a
// strictly smaller Dist (RestrictedPathChildTopology's own weaker condition,
// precedented by NumberOfRestrictedPathsFromFirstToLastNodeTests, counts a
// different, looser family of "monotonically-closer" walks that need not be
// shortest at all - this problem counts only shortest paths). Every edge weight is
// positive, so Dist still strictly decreases along every accepted edge, which is
// what keeps this acyclic and a valid IDagTopology witness.
internal readonly struct WaysChildTopology : IDagTopology<WaysNode, ListChildren<WaysNode>>
{
    public static ListChildren<WaysNode> GetChildren(WaysNode node)
    {
        var children = new List<WaysNode>();

        foreach (var (weight, target) in node.Edges)
        {
            if (node.Dist - weight == target.Dist)
            {
                children.Add(target);
            }
        }

        return new ListChildren<WaysNode>(children);
    }
}
