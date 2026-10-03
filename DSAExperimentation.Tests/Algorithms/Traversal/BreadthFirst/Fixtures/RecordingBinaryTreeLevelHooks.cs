using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;

// RecordingLevelHooks over a BinaryTreeNode, recording each level's values.
internal readonly struct RecordingBinaryTreeLevelHooks<TValue>(List<(int Depth, List<TValue> Values)> levels)
    : ILevelGroupedHooks<BinaryTreeNode<TValue>>
{
    public void OnLevel(IReadOnlyList<BinaryTreeNode<TValue>> level, int depth) =>
        levels.Add((depth, level.Select(n => n.Value).ToList()));
}
