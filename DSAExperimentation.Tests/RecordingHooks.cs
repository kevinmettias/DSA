using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.

public readonly struct RecordingEnterHooks<TMarker> : IDepthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> entered = [];

    public static IReadOnlyList<(string Name, int Depth)> Entered => entered;

    public static void Enter(TestNode node, int depth) => entered.Add((node.Name, depth));
}

public readonly struct RecordingExitHooks<TMarker> : IDepthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> exited = [];

    public static IReadOnlyList<(string Name, int Depth)> Exited => exited;

    public static void Exit(TestNode node, int depth) => exited.Add((node.Name, depth));
}

public readonly struct RecordingEnterExitHooks<TMarker> : IDepthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> entered = [];
    private static readonly List<(string Name, int Depth)> exited = [];

    public static IReadOnlyList<(string Name, int Depth)> Entered => entered;
    public static IReadOnlyList<(string Name, int Depth)> Exited => exited;

    public static void Enter(TestNode node, int depth) => entered.Add((node.Name, depth));
    public static void Exit(TestNode node, int depth) => exited.Add((node.Name, depth));
}

public readonly struct RecordingVisitHooks<TMarker> : IBreadthFirstHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> visited = [];

    public static IReadOnlyList<(string Name, int Depth)> Visited => visited;

    public static void Visit(TestNode node, int depth) => visited.Add((node.Name, depth));
}

public readonly struct RecordingLevelHooks<TMarker> : ILevelGroupedHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(int Depth, List<string> Names)> levels = [];

    public static IReadOnlyList<(int Depth, IReadOnlyList<string> Names)> Levels
        => levels.Select(l => (l.Depth, (IReadOnlyList<string>)l.Names)).ToList();

    public static void OnLevel(IReadOnlyList<TestNode> level, int depth)
        => levels.Add((depth, level.Select(n => n.Name).ToList()));
}
