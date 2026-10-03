using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// RecordingExitHooks over a BinaryTreeNode, recording each node's value.
internal readonly struct RecordingBinaryTreeExitHooks<TValue>(List<(TValue Value, int Depth)> exited)
    : IDepthFirstHooks<BinaryTreeNode<TValue>>
{
    public void Enter(BinaryTreeNode<TValue> node, int depth)
    {
    }

    public void Exit(BinaryTreeNode<TValue> node, int depth) => exited.Add((node.Value, depth));
}
