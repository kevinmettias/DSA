using DSAExperimentation.Algorithms.Graph.Engines.Traversal.DepthFirst;
using DSAExperimentation.Tests.Graph.Fixtures;

namespace DSAExperimentation.Tests.Graph.Engines.Fixtures;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.
internal readonly struct RecordingEnterHooks<TMarker> : IDepthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> Log = [];

    public static IReadOnlyList<(string Name, int Depth)> Entered => Log;

    public static void Enter(TestNode node, int depth) => Log.Add((node.Name, depth));
}
