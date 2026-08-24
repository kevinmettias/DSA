using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

// TMarker isolates static storage per test (a fresh nested marker type gives each
// test its own backing list even though the hook type is otherwise identical), so
// tests can run in parallel without sharing state.
public readonly struct RecordingLevelHooks<TMarker> : ILevelGroupedHooks<TestNode>
    where TMarker : struct
{
    private static readonly List<(int Depth, List<string> Names)> levels = [];

    public static IReadOnlyList<(int Depth, IReadOnlyList<string> Names)> Levels
        => levels.Select(l => (l.Depth, (IReadOnlyList<string>)l.Names)).ToList();

    public static void OnLevel(IReadOnlyList<TestNode> level, int depth)
        => levels.Add((depth, level.Select(n => n.Name).ToList()));
}
