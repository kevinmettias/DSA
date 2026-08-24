using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.
public readonly struct RecordingExitHooks<TMarker> : IDepthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> exited = [];

    public static IReadOnlyList<(string Name, int Depth)> Exited => exited;

    public static void Exit(TestNode node, int depth) => exited.Add((node.Name, depth));
}
