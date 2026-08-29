using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;

// TMarker isolates static storage per test - same precedent as RecordingLevelHooks.
internal readonly struct RecordingBinaryTreeLevelHooks<TValue, TMarker> : ILevelGroupedHooks<BinaryTreeNode<TValue>>
    where TMarker : struct
{
    private static readonly List<(int Depth, List<TValue> Values)> Log = [];

    public static IReadOnlyList<(int Depth, IReadOnlyList<TValue> Values)> Levels
        => Log.Select(l => (l.Depth, (IReadOnlyList<TValue>)l.Values)).ToList();

    public static void OnLevel(IReadOnlyList<BinaryTreeNode<TValue>> level, int depth)
        => Log.Add((depth, level.Select(n => n.Value).ToList()));
}
