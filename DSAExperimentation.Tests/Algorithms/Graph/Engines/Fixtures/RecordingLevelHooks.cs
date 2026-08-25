using DSAExperimentation.Algorithms.Graph.Engines.Traversal.BreadthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Graph.Engines.Fixtures;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.
internal readonly struct RecordingLevelHooks<TMarker> : ILevelGroupedHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(int Depth, List<string> Names)> Log = [];

    public static IReadOnlyList<(int Depth, IReadOnlyList<string> Names)> Levels
        => Log.Select(l => (l.Depth, (IReadOnlyList<string>)l.Names)).ToList();

    public static void OnLevel(IReadOnlyList<TestNode> level, int depth)
        => Log.Add((depth, level.Select(n => n.Name).ToList()));
}
