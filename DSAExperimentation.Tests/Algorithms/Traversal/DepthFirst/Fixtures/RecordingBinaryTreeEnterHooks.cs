using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// RecordingEnterHooks over a BinaryTreeNode, recording each node's value.
internal readonly struct RecordingBinaryTreeEnterHooks<TValue>(List<(TValue Value, int Depth)> entered)
    : IDepthFirstHooks<BinaryTreeNode<TValue>>
{
    public void Enter(BinaryTreeNode<TValue> node, int depth) => entered.Add((node.Value, depth));

    public void Exit(BinaryTreeNode<TValue> node, int depth)
    {
    }
}
