using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfVisitedCellsInAGrid.Fixtures;

internal readonly struct JumpGridTopology : IGraphTopology<JumpGridNode, JumpGridChildren>
{
    public static JumpGridChildren GetChildren(JumpGridNode node) => new(node);
}
