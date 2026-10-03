using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;

// Counts by value - see CountingEnterExitHooks: the count is visible only in the hook value
// a walk hands back.
internal struct CountingVisitHooks : IBreadthFirstHooks<TestNode>
{
    public int Visited { get; private set; }

    public void Visit(TestNode node, int depth) => Visited++;
}
