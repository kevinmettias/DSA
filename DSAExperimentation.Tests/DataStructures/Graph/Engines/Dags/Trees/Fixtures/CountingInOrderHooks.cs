using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

// Counts by value rather than through a reference it holds, so the count is visible only in the
// hook value Walk hands back - which shows every visit acted on that one value, not on a copy a
// recursive frame was handed.
internal struct CountingInOrderHooks : IInOrderHooks<int>
{
    public int Visited { get; private set; }

    public void Visit(BinaryTreeNode<int> node, int depth) => Visited++;
}
