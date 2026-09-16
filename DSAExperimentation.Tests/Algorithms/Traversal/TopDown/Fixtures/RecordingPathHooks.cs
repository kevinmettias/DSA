using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.TopDown.Fixtures;

// State is the root-to-node path so far, so a recorded entry proves both that the
// node was visited and that Descend built its state from its own parent's - not
// from a sibling's, which is the distinction ITopDownHooks exists to make.
internal readonly struct RecordingPathHooks
    : ITopDownHooks<TestNode, RecordingPathHooks.PathSoFar>
{
    public static void Visit(TestNode node, PathSoFar state, int depth, NodePosition position) =>
        state.Recorded.Add((string.Join("/", state.Names), depth, position));

    public static PathSoFar Descend(TestNode parent, PathSoFar parentState, TestNode child) =>
        parentState with { Names = [.. parentState.Names, child.Name] };

    // The state the hooks thread down, nested at the end of the hook rather than
    // left at file scope so the file declares exactly one type. The base clause
    // above qualifies it - a base clause is resolved outside the type's own body,
    // so an unqualified PathSoFar there would be CS0246 - and so must every caller
    // outside this file (TopDownWalkTests, TopDownTraversalTests): the namespace
    // using they already have no longer reaches a nested type.
    internal readonly record struct PathSoFar(
        List<string> Names,
        List<(string Path, int Depth, NodePosition Position)> Recorded);
}
