using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// TMarker isolates static storage per test - same precedent as RecordingEnterHooks.
internal readonly struct RecordingBinaryTreeEnterHooks<TValue, TMarker> : IDepthFirstHooks<BinaryTreeNode<TValue>>
    where TMarker : struct
{
    private static readonly List<(TValue Value, int Depth)> Log = [];

    public static IReadOnlyList<(TValue Value, int Depth)> Entered => Log;

    public static void Enter(BinaryTreeNode<TValue> node, int depth) => Log.Add((node.Value, depth));
}
