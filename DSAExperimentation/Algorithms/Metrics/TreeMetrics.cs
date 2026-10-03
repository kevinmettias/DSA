using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;

namespace DSAExperimentation.Algorithms.Metrics;

internal static class TreeMetrics
{
    public static int Size<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, SizeAlgebra<TNode>, int>(root);

    // In the topology's own child order.
    public static int Size<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Size<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);

    public static int Height<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, HeightAlgebra<TNode>, int>(root);

    // In the topology's own child order.
    public static int Height<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Height<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);

    public static int Diameter<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            DiameterAlgebra<TNode>, HeightDiameterState>(root).Diameter;

    // In the topology's own child order.
    public static int Diameter<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Diameter<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);

    // One traversal for both, via ZipFoldAlgebra, instead of calling Height and
    // Size separately.
    public static (int Height, int Size) HeightAndSize<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<
            TNode,
            TTopology,
            TChildren,
            TOrder,
            TOrderedChildren,
            ZipFoldAlgebra<TNode, int, int, HeightAlgebra<TNode>, SizeAlgebra<TNode>>,
            (int, int)>(root);

    // In the topology's own child order.
    public static (int Height, int Size) HeightAndSize<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => HeightAndSize<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);
}
