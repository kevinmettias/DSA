using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state - same precedent as
// RecordingEnterHooks/RecordingExitHooks/RecordingLevelHooks.
internal readonly struct RecordingInOrderHooks<TValue, TMarker> : IInOrderHooks<TValue>
    where TMarker : struct
{
    private static readonly List<(TValue Value, int Depth)> Log = [];

    public static IReadOnlyList<(TValue Value, int Depth)> Visited => Log;

    public static void Visit(BinaryTreeNode<TValue> node, int depth) => Log.Add((node.Value, depth));
}
