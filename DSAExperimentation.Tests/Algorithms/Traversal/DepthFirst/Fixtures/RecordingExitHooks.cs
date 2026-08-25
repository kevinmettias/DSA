using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.
internal readonly struct RecordingExitHooks<TMarker> : IDepthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> Log = [];

    public static IReadOnlyList<(string Name, int Depth)> Exited => Log;

    public static void Exit(TestNode node, int depth) => Log.Add((node.Name, depth));
}
