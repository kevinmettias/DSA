using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;

// Counts by value - see CountingVisitHooks: the count is visible only in the hook value the
// walk hands back.
internal struct CountingLevelHooks : ILevelGroupedHooks<TestNode>
{
    public int Levels { get; private set; }

    public void OnLevel(IReadOnlyList<TestNode> level, int depth) => Levels++;
}
