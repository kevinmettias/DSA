using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

// Appends each visited node's value and depth to the list it was built with, which each test
// builds for itself, so no two tests share one.
internal readonly struct RecordingInOrderHooks<TValue>(List<(TValue Value, int Depth)> visited) : IInOrderHooks<TValue>
{
    public void Visit(BinaryTreeNode<TValue> node, int depth) => visited.Add((node.Value, depth));
}
