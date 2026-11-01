using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Traversal.BreadthFirst;

// Genuinely distinct from BreadthFirstTraversal, not just a
// convenience wrapper over it: this buffers an entire depth before firing, since a
// level isn't known to be complete until every node at that depth has been
// discovered. Shares BreadthFirstWalk's frontier computation (NextLevel) rather than
// reimplementing the enqueue/dequeue mechanics - the two differ only in what happens
// to a level once it's known: BreadthFirstWalk steps through it node by node, this
// fires once for the whole buffered list.
/// <summary>A breadth-first walk over a tree that hands a hook each whole level at once.</summary>
public static class LevelGroupedBreadthFirstTraversal
{
    /// <summary>
    /// Walks a tree one level at a time in <c>TOrder</c>'s child order, starting from the given hook value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> for an empty tree.</param>
    /// <param name="hooks">The hook value the walk starts from.</param>
    /// <returns>
    /// The hook value the walk finished with, or <paramref name="hooks"/> unchanged when <paramref
    /// name="root"/> is <c>null</c>.
    /// </returns>
    public static THooks Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root, THooks hooks)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, ILevelGroupedHooks<TNode>
    {
        if (root is null)
        {
            return hooks;
        }

        var currentLevel = new List<TNode> { root };
        var depth = 0;

        while (currentLevel.Count > 0)
        {
            hooks.OnLevel(currentLevel, depth);

            currentLevel = BreadthFirstWalk.NextLevel<
                TNode, TTopology, TChildren, TOrder, TOrderedChildren, UnguardedVisit<TNode>>(
                currentLevel, default);
            depth++;
        }

        return hooks;
    }

    // In the topology's own child order.
    /// <summary>
    /// Walks a tree one level at a time in the topology's own child order, starting from the given hook value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> for an empty tree.</param>
    /// <param name="hooks">The hook value the walk starts from.</param>
    /// <returns>
    /// The hook value the walk finished with, or <paramref name="hooks"/> unchanged when <paramref
    /// name="root"/> is <c>null</c>.
    /// </returns>
    public static THooks Walk<TNode, TTopology, TChildren, THooks>(TNode? root, THooks hooks)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, ILevelGroupedHooks<TNode>
        => Walk<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root, hooks);
}
