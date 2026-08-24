using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.
public readonly struct RecordingVisitHooks<TMarker> : IBreadthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> visited = [];

    public static IReadOnlyList<(string Name, int Depth)> Visited => visited;

    public static void Visit(TestNode node, int depth) => visited.Add((node.Name, depth));
}
