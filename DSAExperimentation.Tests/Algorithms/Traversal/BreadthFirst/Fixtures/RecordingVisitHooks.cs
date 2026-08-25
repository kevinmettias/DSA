using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.
internal readonly struct RecordingVisitHooks<TMarker> : IBreadthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> Log = [];

    public static IReadOnlyList<(string Name, int Depth)> Visited => Log;

    public static void Visit(TestNode node, int depth) => Log.Add((node.Name, depth));
}
