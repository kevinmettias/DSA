using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding.Fixtures;

// Combine spells a subtree as its root's name followed by its children's results in the
// order Combine received them, so the folded string shows the child order a fold
// delivered. Enter records every (name, depth) it is handed, which a fold's result
// cannot show. TMarker isolates that static log per test, the same way the recording
// traversal hooks do.
internal readonly struct RecordingNamesFoldAlgebra<TMarker> : IFoldAlgebra<TestNode, string>
    where TMarker : struct
{
    private static readonly List<(string Name, int Depth)> EnterLog = [];

    public static IReadOnlyList<(string Name, int Depth)> Entered => EnterLog;

    public static string Empty => "";

    public static void Enter(TestNode node, int depth) => EnterLog.Add((node.Name, depth));

    public static string Combine(TestNode node, IReadOnlyList<string> children)
        => node.Name + string.Concat(children);
}
