using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// Appends each entered node and its depth to the list it was built with, which each test
// builds for itself, so no two tests share one. Exit is left empty: a pre-order hook.
internal readonly struct RecordingEnterHooks(List<(string Name, int Depth)> entered) : IDepthFirstHooks<TestNode>
{
    public void Enter(TestNode node, int depth) => entered.Add((node.Name, depth));

    public void Exit(TestNode node, int depth)
    {
    }
}
