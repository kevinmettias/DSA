using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// Appends each exited node and its depth to the list it was built with, which each test
// builds for itself, so no two tests share one. Enter is left empty: a post-order hook.
internal readonly struct RecordingExitHooks(List<(string Name, int Depth)> exited) : IDepthFirstHooks<TestNode>
{
    public void Enter(TestNode node, int depth)
    {
    }

    public void Exit(TestNode node, int depth) => exited.Add((node.Name, depth));
}
