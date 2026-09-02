using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Benchmarks.Fixtures;

// DiameterAlgebra generalized with a per-edge validity check: a child only
// contributes to the two tallest heights (the path through this node) when its own
// label differs from this node's. Mirrors the Tests project's own LongestPathAlgebra
// exactly; see LongestPathWithDifferentAdjacentCharactersBenchmarks for the O(n^2)
// naive-recompute variant this is benchmarked against.
internal readonly struct LongestPathAlgebra : IFoldAlgebra<PathNode, PathState>
{
    public static PathState Empty => new(0, 1, '\0');

    // Accumulates the running largest-height, second-largest-height, and
    // largest-child-path values as Combine folds over one node's children.
    private readonly record struct ChildAccumulator(int LargestHeight, int SecondLargestHeight, int LargestChildPath);

    public static PathState Combine(PathNode node, IReadOnlyList<PathState> children)
    {
        var accumulator = new ChildAccumulator(0, 0, 1);

        for (var i = 0; i < children.Count; i++)
        {
            accumulator = AccumulateChild(node, children[i], accumulator);
        }

        var height = 1 + accumulator.LargestHeight;
        var pathThroughNode = 1 + accumulator.LargestHeight + accumulator.SecondLargestHeight;

        return new PathState(height, Math.Max(pathThroughNode, accumulator.LargestChildPath), node.Label);
    }

    // Folds a single child into the running accumulator: its own longest path
    // always contributes, but its height only folds into the two tallest
    // heights seen so far when the edge to it is valid (labels differ).
    private static ChildAccumulator AccumulateChild(PathNode node, PathState child, ChildAccumulator accumulator)
    {
        var largestChildPath = Math.Max(child.LongestPath, accumulator.LargestChildPath);

        if (child.Label == node.Label)
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
