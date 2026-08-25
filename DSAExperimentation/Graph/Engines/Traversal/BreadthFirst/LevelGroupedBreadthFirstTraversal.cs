using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Dags.Trees;
using DSAExperimentation.Graph.Engines.Walking;

namespace DSAExperimentation.Graph.Engines.Traversal.BreadthFirst;

// Genuinely distinct from BreadthFirstTraversal<..., NodeVisit<...>>, not just a
// convenience wrapper over it: this buffers an entire depth before firing, since a
// level isn't known to be complete until every node at that depth has been
// discovered. Shares BreadthFirstWalk's frontier computation (NextLevel) rather than
// reimplementing the enqueue/dequeue mechanics - the two differ only in what happens
// to a level once it's known: BreadthFirstWalk steps through it node by node, this
// fires once for the whole buffered list.
internal static class LevelGroupedBreadthFirstTraversal
{
    public static void Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, ILevelGroupedHooks<TNode>
    {
        if (root is null)
        {
            return;
        }

        var currentLevel = new List<TNode> { root };
        var depth = 0;

        while (currentLevel.Count > 0)
        {
            THooks.OnLevel(currentLevel, depth);

            currentLevel = BreadthFirstWalk.NextLevel<
                TNode, TTopology, TChildren, TOrder, TOrderedChildren, UnguardedVisit<TNode>>(
                currentLevel, default);
            depth++;
        }
    }
}
