using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;

namespace DSAExperimentation.Algorithms.Metrics;

/// <summary>A tree's size, height and diameter, each computed in one fold.</summary>
public static class TreeMetrics
{
    /// <summary>Counts a tree's nodes, folding in <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The root of the tree, or null for an empty tree.</param>
    /// <returns>The number of nodes, 0 for a null root.</returns>
    public static int Size<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, SizeAlgebra<TNode>, int>(root);

    // In the topology's own child order.
    /// <summary>Counts a tree's nodes, folding in the topology's own child order.</summary>
    /// <param name="root">The root of the tree, or null for an empty tree.</param>
    /// <returns>The number of nodes, 0 for a null root.</returns>
    public static int Size<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Size<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);

    /// <summary>Measures a tree's height in nodes, folding in <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The root of the tree, or null for an empty tree.</param>
    /// <returns>The number of nodes on the longest path down from the root, 0 for a null root.</returns>
    public static int Height<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => TreeFold.Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, HeightAlgebra<TNode>, int>(root);

    // In the topology's own child order.
    /// <summary>Measures a tree's height in nodes, folding in the topology's own child order.</summary>
    /// <param name="root">The root of the tree, or null for an empty tree.</param>
    /// <returns>The number of nodes on the longest path down from the root, 0 for a null root.</returns>
    public static int Height<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Height<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);

    /// <summary>Measures a tree's diameter in edges, folding in <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The root of the tree, or null for an empty tree.</param>
    /// <returns>The number of edges on the longest path between two nodes, 0 for a null root.</returns>
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
    /// <summary>Measures a tree's diameter in edges, folding in the topology's own child order.</summary>
    /// <param name="root">The root of the tree, or null for an empty tree.</param>
    /// <returns>The number of edges on the longest path between two nodes, 0 for a null root.</returns>
    public static int Diameter<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Diameter<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);

    // One traversal for both, via ZipFoldAlgebra, instead of calling Height and
    // Size separately.
    /// <summary>Finds a tree's height and size together, folding in <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The root of the tree, or null for an empty tree.</param>
    /// <returns>The height in nodes and the number of nodes, both 0 for a null root.</returns>
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
    /// <summary>Finds a tree's height and size together, folding in the topology's own child order.</summary>
    /// <param name="root">The root of the tree, or null for an empty tree.</param>
    /// <returns>The height in nodes and the number of nodes, both 0 for a null root.</returns>
    public static (int Height, int Size) HeightAndSize<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => HeightAndSize<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);
}
