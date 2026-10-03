using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;

// Appends each node and its depth to one list as it is entered and to the other as it is
// exited; each test builds both lists for itself, so no two tests share one.
internal readonly struct RecordingEnterExitHooks(
    List<(string Name, int Depth)> entered, List<(string Name, int Depth)> exited) : IDepthFirstHooks<TestNode>
{
    public void Enter(TestNode node, int depth) => entered.Add((node.Name, depth));

    public void Exit(TestNode node, int depth) => exited.Add((node.Name, depth));
}
