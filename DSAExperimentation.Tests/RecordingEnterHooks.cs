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
