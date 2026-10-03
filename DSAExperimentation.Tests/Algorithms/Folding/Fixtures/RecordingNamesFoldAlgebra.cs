using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding.Fixtures;

// Combine spells a subtree as its root's name followed by its children's results in the
// order Combine received them, so the folded string shows the child order a fold
// delivered. It also logs each node's name as that node is combined, which the folded
// string cannot show: the order a fold combined its nodes in, and how many times it
// combined each one. TMarker isolates that static log per test, the same way the
// recording traversal hooks do.
internal readonly struct RecordingNamesFoldAlgebra<TMarker> : IFoldAlgebra<TestNode, string>
    where TMarker : struct
{
    private static readonly List<string> CombineLog = [];

    public static IReadOnlyList<string> Combined => CombineLog;

    public static string Empty => "";

    public static string Combine(TestNode node, IReadOnlyList<string> children)
    {
        CombineLog.Add(node.Name);

        return node.Name + string.Concat(children);
    }
}
