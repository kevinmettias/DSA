using DSAExperimentation.Graph.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Folding;

namespace DSAExperimentation.Graph.Engines.Dags.Trees;

internal static class TreeMetrics
{
    public static int Size<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, SizeAlgebra<TNode>, int>(root);

    public static int Height<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, HeightAlgebra<TNode>, int>(root);

    public static int Diameter<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            DiameterAlgebra<TNode>, HeightDiameterState>(root).Diameter;

    // One traversal for both, via ZipFoldAlgebra, instead of calling Height and
    // Size separately.
    public static (int Height, int Size) HeightAndSize<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var (height, size) = TreeFold.Fold<
            TNode,
            TTopology,
            TChildren,
            TOrder,
            TOrderedChildren,
            ZipFoldAlgebra<TNode, int, int, HeightAlgebra<TNode>, SizeAlgebra<TNode>>,
            (int, int)>(root);

        return (height, size);
    }
}
