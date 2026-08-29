using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// TMarker isolates static storage per test - same precedent as RecordingExitHooks.
internal readonly struct RecordingBinaryTreeExitHooks<TValue, TMarker> : IDepthFirstHooks<BinaryTreeNode<TValue>>
    where TMarker : struct
{
    private static readonly List<(TValue Value, int Depth)> Log = [];

    public static IReadOnlyList<(TValue Value, int Depth)> Exited => Log;

    public static void Exit(BinaryTreeNode<TValue> node, int depth) => Log.Add((node.Value, depth));
}
