using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Contracts.Topologies;
using DSAExperimentation.Graph.Engines.Traversal.TopDown;

namespace DSAExperimentation.Graph.Engines.Walking;

// The engine behind ITopDownHooks - see DepthFirstWalk for the shared-engine
// reasoning (topology-agnostic, TGuard as the one tree-vs-graph axis). Structurally
// this is closer to DepthFirstWalk than BreadthFirstWalk (recursive, depth-first
// discovery), but it is not a third overload of DepthFirstWalk: that engine threads
// ONE state value sequentially across the whole walk (a child's Exit-state becomes
// the next sibling's Enter-input - see the comment on DepthFirstWalk itself), which
// is exactly wrong here. This engine computes a FRESH state per child from its
// parent's state (Descend), independent of what any sibling subtree does - an
// inherited attribute, not a synthesized one. No Exit, no return value: nothing
// flows back up, only down and across via Visit's side effects (see
// AllRootToLeafPaths for how a caller gets a result out of that).
internal static class TopDownWalk
{
    internal static void Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, THooks, TState>(
        TNode node, TState state, int depth, TGuard guard)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : IVisitGuard<TNode>
        where THooks : struct, ITopDownHooks<TNode, TState>
    {
        var children = TOrder.Apply(TTopology.GetChildren(node));

        THooks.Visit(node, state, depth, children.Count == 0 ? NodePosition.Leaf : NodePosition.Interior);

        for (var i = 0; i < children.Count; i++)
        {
            var child = children.Get(i);

            if (guard.ShouldVisit(child))
            {
                Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, THooks, TState>(
                    child, THooks.Descend(node, state, child), depth + 1, guard);
            }
        }
    }
}
