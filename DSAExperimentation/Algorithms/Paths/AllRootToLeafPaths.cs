using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Traversal.TopDown;

namespace DSAExperimentation.Algorithms.Paths;

// A canonical proof for ITopDownHooks: every root-to-leaf path is naturally
// top-down (a path is exactly "the state a node inherited from its parent, plus
// itself") and awkward bottom-up - a Fold's Combine only ever sees a node together
// with its children's *finished* results, with no way to hand each child a copy of
// "the path so far" before that child even starts.
//
// The output list travels as part of TState itself (never copied, same reference at
// every node) rather than through a static mutable field - the same "state carries a
// reference to shared context" pattern GridNode uses for its Grid.
/// <summary>Every root-to-leaf path of a tree, each an array of the nodes from the root down.</summary>
public static class AllRootToLeafPaths
{
    /// <summary>Lists every root-to-leaf path, walking children in the order TOrder gives them.</summary>
    /// <param name="root">The tree's root, or null for an empty tree.</param>
    /// <returns>
    /// One array of its own per leaf, root first, in the order a depth-first walk reaches the leaves; empty
    /// when root is null.
    /// </returns>
    public static List<TNode[]> Find<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var output = new List<TNode[]>();

        TopDownTraversal.Walk<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            CollectPathsHooks<TNode>, (TNode[] Path, List<TNode[]> Output)>(
            root, (Path: root is null ? EmptyPath<TNode>() : SingletonPath(root), Output: output));

        return output;
    }

    // In the topology's own child order.
    /// <summary>Lists every root-to-leaf path, walking children in the topology's own order.</summary>
    /// <param name="root">The tree's root, or null for an empty tree.</param>
    /// <returns>
    /// One array of its own per leaf, root first, in the order a depth-first walk reaches the leaves; empty
    /// when root is null.
    /// </returns>
    public static List<TNode[]> Find<TNode, TTopology, TChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Find<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root);

    private static TNode[] SingletonPath<TNode>(TNode root)
        where TNode : class
        => [root];

    private static TNode[] EmptyPath<TNode>()
        where TNode : class
        => [];

    private readonly struct CollectPathsHooks<TNode> : ITopDownHooks<TNode, (TNode[] Path, List<TNode[]> Output)>
        where TNode : class
    {
        public static void Visit(TNode node, (TNode[] Path, List<TNode[]> Output) state, int depth, NodePosition position)
        {
            if (position == NodePosition.Leaf)
            {
                state.Output.Add(state.Path);
            }
        }

        public static (TNode[] Path, List<TNode[]> Output) Descend(
            TNode parent, (TNode[] Path, List<TNode[]> Output) parentState, TNode child)
        {
            var path = new TNode[parentState.Path.Length + 1];
            Array.Copy(parentState.Path, path, parentState.Path.Length);
            path[^1] = child;

            return (path, parentState.Output);
        }
    }
}
