using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfRestrictedPathsFromFirstToLastNode.Fixtures;

// A restricted path only ever steps to a neighbor with a strictly smaller distance
// to the last node, so this can never revisit a node already on the walk - exactly
// IDagTopology's acyclicity promise, earned by construction (Dist strictly
// decreases) rather than checked at runtime.
internal readonly struct RestrictedPathChildTopology
    : IDagTopology<RestrictedPathNode, ListChildren<RestrictedPathNode>>
{
    public static ListChildren<RestrictedPathNode> GetChildren(RestrictedPathNode node)
    {
        var children = new List<RestrictedPathNode>();

        foreach (var (_, target) in node.Edges)
        {
            if (target.Dist < node.Dist)
            {
                children.Add(target);
            }
        }

        return new ListChildren<RestrictedPathNode>(children);
    }
}
