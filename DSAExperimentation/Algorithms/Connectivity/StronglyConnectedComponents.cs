using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.Connectivity;

// Tarjan's algorithm: one recursive DFS pass assigns each node a discovery index and a
// low-link value (the lowest discovery index reachable from it via tree edges plus at most
// one back edge), and a node whose low-link never drops below its own discovery index is the
// root of a strongly connected component - everything still on the call stack above it
// belongs to that same component. Needs only IGraphTopology, not IDagTopology - the whole
// point is finding the cycles a DAG promise would rule out.
//
// TarjanState.DiscoveryIndex.Count doubles as the running discovery counter (index assigned =
// count before insertion), so no separate mutable counter needs threading through the
// recursion. The five pieces of per-call mutable state are bundled into one record and passed
// by reference rather than as five separate parameters - CheckedFold.cs's own recursion
// threads its two scratch structures (completed, inProgress) as bare parameters, but that
// shape hits this gate's own check-parameter-count limit once a fifth and sixth piece of
// state (onStack, callStack, components) join discoveryIndex/lowLink, so this bundles instead.
//
// Real C# recursion, not an explicit stack, matching DepthFirstWalk.cs's engine and
// CheckedFold.cs's own "deliberately recursive-only" precedent: DepthFirstWalk's
// IVisitGuard/IDepthFirstHooks engine can't be reused here regardless, since it has no
// per-child-result callback and this needs to fold a child's low-link into its parent's
// immediately after that child's recursive call returns. The SCC-accumulation stack is a
// plain BCL Stack<TNode>, not this repo's own DataStructures/Stack - that type is reserved
// for substituting the literal call stack of an iterative traversal (DepthFirstSearch.cs's
// only use of it), and recursion already provides that here; this stack is just
// Operations-internal bookkeeping, the same bucket CheckedFold's HashSet/Dictionary occupy.
//
// Unenforced precondition, the same self-discovering shape ConnectedComponents.Count relies
// on (undocumented there, stated explicitly here): nodes must include a representative from
// every component not reachable from another node already listed, or that component is
// silently absent from the result - there is no reverse-adjacency contract anywhere in
// Graph/Contracts that could notice a missing one.
internal static class StronglyConnectedComponents
{
    public static List<List<TNode>> Tarjan<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        IEnumerable<TNode> nodes)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var state = new TarjanState<TNode>(new(), new(), new(), new(), new());

        foreach (var node in nodes)
        {
            if (!state.DiscoveryIndex.ContainsKey(node))
            {
                StrongConnect<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(node, state);
            }
        }

        return state.Components;
    }

    private static void StrongConnect<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, TarjanState<TNode> state)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        Initialize(node, state);
        VisitChildren<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(node, state);

        if (state.LowLink[node] == state.DiscoveryIndex[node])
        {
            var component = DrainComponent(node, state);
            state.Components.Add(component);
        }
    }

    private static void Initialize<TNode>(TNode node, TarjanState<TNode> state)
        where TNode : class
    {
        var index = state.DiscoveryIndex.Count;
        state.DiscoveryIndex[node] = index;
        state.LowLink[node] = index;
        state.CallStack.Push(node);
        state.OnStack.Add(node);
    }

    private static List<TNode> DrainComponent<TNode>(TNode root, TarjanState<TNode> state)
        where TNode : class
    {
        var component = new List<TNode>();
        TNode member;

        do
        {
            member = state.CallStack.Pop();
            state.OnStack.Remove(member);
            component.Add(member);
        } while (!ReferenceEquals(member, root));

        return component;
    }

    private static void VisitChildren<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, TarjanState<TNode> state)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var children = TOrder.Apply(TTopology.GetChildren(node));

        for (var i = 0; i < children.Count; i++)
        {
            var child = children.Get(i);

            if (!state.DiscoveryIndex.ContainsKey(child))
            {
                StrongConnect<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(child, state);
                state.LowLink[node] = Math.Min(state.LowLink[node], state.LowLink[child]);
            }
            else if (state.OnStack.Contains(child))
            {
                state.LowLink[node] = Math.Min(state.LowLink[node], state.DiscoveryIndex[child]);
            }
        }
    }

    private sealed record TarjanState<TNode>(
        Dictionary<TNode, int> DiscoveryIndex,
        Dictionary<TNode, int> LowLink,
        HashSet<TNode> OnStack,
        Stack<TNode> CallStack,
        List<List<TNode>> Components)
        where TNode : class;
}
