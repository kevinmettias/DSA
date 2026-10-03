using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;

// Appends each visited node and its depth to the list it was built with, which each test
// builds for itself, so no two tests share one.
internal readonly struct RecordingVisitHooks(List<(string Name, int Depth)> visited) : IBreadthFirstHooks<TestNode>
{
    public void Visit(TestNode node, int depth) => visited.Add((node.Name, depth));
}
