using DSAExperimentation.Trees;

namespace DSAExperimentation.Tests;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the action type is otherwise identical),
// so tests can run in parallel without sharing state.

public readonly struct RecordingNodeAction<TMarker> : INodeAction<TestNode>
    where TMarker : struct
{
    private static readonly List<string> visited = [];

    public static IReadOnlyList<string> Visited => visited;

    public static void Invoke(TestNode node) => visited.Add(node.Name);
}

public readonly struct RecordingDepthAwareAction<TMarker> : IDepthAwareNodeAction<TestNode>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> visited = [];

    public static IReadOnlyList<(string Name, int Depth)> Visited => visited;

    public static void Invoke(TestNode node, int depth) => visited.Add((node.Name, depth));
}

public readonly struct RecordingLevelAction<TMarker> : ILevelVisitAction<TestNode>
    where TMarker : struct
{
    private static readonly List<(int Depth, List<string> Names)> levels = [];

    public static IReadOnlyList<(int Depth, IReadOnlyList<string> Names)> Levels
        => levels.Select(l => (l.Depth, (IReadOnlyList<string>)l.Names)).ToList();

    public static void Invoke(IReadOnlyList<TestNode> level, int depth)
        => levels.Add((depth, level.Select(n => n.Name).ToList()));
}

public readonly struct CountNodesFoldAlgebra : IFoldAlgebra<TestNode, int>
{
    public static int Empty => 0;

    public static int Combine(TestNode node, IReadOnlyList<int> children)
        => 1 + children.Sum();
}

public readonly struct CountNodesBreadthFirstAlgebra : IBreadthFirstReduceAlgebra<TestNode, int>
{
    public static int Seed => 0;

    public static int Accumulate(int state, TestNode node, int depth) => state + 1;
}
