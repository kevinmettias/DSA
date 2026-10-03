using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;

// Appends each level's depth and node names to the list it was built with, which each test
// builds for itself, so no two tests share one.
internal readonly struct RecordingLevelHooks(List<(int Depth, List<string> Names)> levels)
    : ILevelGroupedHooks<TestNode>
{
    public void OnLevel(IReadOnlyList<TestNode> level, int depth) =>
        levels.Add((depth, level.Select(n => n.Name).ToList()));
}
