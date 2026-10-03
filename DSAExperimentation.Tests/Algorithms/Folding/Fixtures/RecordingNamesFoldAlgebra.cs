using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding.Fixtures;

// Combine spells a subtree as its root's name followed by its children's results in the
// order Combine received them, so the folded string shows the child order a fold
// delivered. It also appends each node's name to the log it was built with as that node
// is combined, which the folded string cannot show: the order a fold combined its nodes
// in, and how many times it combined each one. Each test builds its own log, so no two
// tests share one.
internal readonly struct RecordingNamesFoldAlgebra(List<string> combined) : IFoldAlgebra<TestNode, string>
{
    public string Empty => "";

    public string Combine(TestNode node, IReadOnlyList<string> children)
    {
        combined.Add(node.Name);

        return node.Name + string.Concat(children);
    }
}
