using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// Counts by value rather than through a reference it holds, so the counts are visible only
// in the hook value a walk hands back - the one thing a hook that appends to a list it was
// given cannot show, since its list fills whichever copy the walk returns.
internal struct CountingEnterExitHooks : IDepthFirstHooks<TestNode>
{
    public int Entered { get; private set; }

    public int Exited { get; private set; }

    public void Enter(TestNode node, int depth) => Entered++;

    public void Exit(TestNode node, int depth) => Exited++;
}
