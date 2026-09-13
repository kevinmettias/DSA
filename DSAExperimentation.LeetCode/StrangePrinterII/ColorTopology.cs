using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.StrangePrinterII;

internal readonly struct ColorTopology : IGraphTopology<ColorNode, ListChildren<ColorNode>>
{
    public static ListChildren<ColorNode> GetChildren(ColorNode node) => new(node.MustPrintBefore);
}
