using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 572 - two left-skewed chains sharing the
// same filler value so the naive per-node comparison can't short-circuit on an
// early mismatch, with the smaller chain's deepest node carrying a value that
// never appears in the larger one, so neither strategy ever finds a real match
// and both run to completion.
internal static class SubtreeOfAnotherTreeWorkloads
{
    public const int SubRootSizeDivisor = 2;

    public static BinaryTreeNode<int> BuildLeftChain(int length, int lastValue)
    {
        var root = new BinaryTreeNode<int>(length == 1 ? lastValue : 1);
        var current = root;

        for (var i = 1; i < length; i++)
        {
            current.Left = new BinaryTreeNode<int>(i == length - 1 ? lastValue : 1);
            current = current.Left;
        }

        return root;
    }
}
