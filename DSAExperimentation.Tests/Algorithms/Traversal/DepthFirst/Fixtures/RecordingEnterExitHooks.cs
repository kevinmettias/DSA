using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.
internal readonly struct RecordingEnterExitHooks<TMarker> : IDepthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> EnterLog = [];
    private static readonly List<(string Name, int Depth)> ExitLog = [];

    public static IReadOnlyList<(string Name, int Depth)> Entered => EnterLog;
    public static IReadOnlyList<(string Name, int Depth)> Exited => ExitLog;

    public static void Enter(TestNode node, int depth) => EnterLog.Add((node.Name, depth));
    public static void Exit(TestNode node, int depth) => ExitLog.Add((node.Name, depth));
}
