using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.LongestPathWithDifferentAdjacentCharacters;

// DiameterAlgebra generalized with a per-edge validity check: a child only
// contributes to the two tallest heights (the path through this node) when its own
// label differs from this node's - an edge with matching labels is simply excluded,
// the same way a missing edge would be, while that child's own best internal path
// (LongestPath) still flows upward untouched via the largest-child-LongestPath scan.
//
// RootedTreeNode carries only a dense id, so the labels are stashed once by
// Prepare before the fold begins and indexed by that id - the same "external state
// established before the walk starts" shape RoomWaysPrecomputedFactorialAlgebra
// uses for its factorial table, and the only shape a static-abstract algebra
// admits. They sit in an AsyncLocal rather than in a plain static field so the
// stash is owned by the flow that called Prepare: a second fold running on another
// thread indexes its own labels, never this one's. This algebra answers one
// LeetCode problem and nothing else, which is why it lives beside the solution
// rather than in Domain.
internal readonly struct LongestPathAlgebra : IFoldAlgebra<RootedTreeNode, PathState>
{
    // No node carries this label, so an Empty state never matches a real one.
    private const char NoLabel = '\0';

    private static readonly AsyncLocal<string> Labels = new();

    public static PathState Empty => new(0, 1, NoLabel);

    // Accumulates the running largest-height, second-largest-height, and
    // largest-child-path values as Combine folds over one node's children.
    private readonly record struct ChildAccumulator(int LargestHeight, int SecondLargestHeight, int LargestChildPath);

    // labels[i] is node i's character; ParentArrayTree numbers its nodes by the same
    // index, so one lookup by id is all Combine needs.
    public static void Prepare(string labels) => Labels.Value = labels;

    public static PathState Combine(RootedTreeNode node, IReadOnlyList<PathState> children)
    {
        var label = Labels.Value[node.Id];
        var accumulator = new ChildAccumulator(0, 0, 1);

        for (var i = 0; i < children.Count; i++)
        {
            accumulator = AccumulateChild(label, children[i], accumulator);
        }

        var height = 1 + accumulator.LargestHeight;
        var pathThroughNode = 1 + accumulator.LargestHeight + accumulator.SecondLargestHeight;

        return new PathState(height, Math.Max(pathThroughNode, accumulator.LargestChildPath), label);
    }

    // Folds a single child into the running accumulator: its own longest path
    // always contributes, but its height only folds into the two tallest
    // heights seen so far when the edge to it is valid (labels differ).
    private static ChildAccumulator AccumulateChild(char label, PathState child, ChildAccumulator accumulator)
    {
        var largestChildPath = Math.Max(child.LongestPath, accumulator.LargestChildPath);

        if (child.Label == label)
        {
            return accumulator with { LargestChildPath = largestChildPath };
        }

        var (largestHeight, secondLargestHeight) = FoldHeight(
            child.Height, accumulator.LargestHeight, accumulator.SecondLargestHeight);

        return new ChildAccumulator(largestHeight, secondLargestHeight, largestChildPath);
    }

    // Folds one more height into the running two tallest heights seen so far.
    private static (int LargestHeight, int SecondLargestHeight) FoldHeight(
        int childHeight, int largestHeight, int secondLargestHeight)
    {
        if (childHeight > largestHeight)
        {
            return (childHeight, largestHeight);
        }

        return (largestHeight, Math.Max(childHeight, secondLargestHeight));
    }
}
