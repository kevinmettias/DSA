using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.Bipartiteness;

// Multi-root BFS 2-coloring: a graph is bipartite iff every edge connects nodes of opposite
// color, which a level-by-level BFS can assign greedily and verify in the same pass - the
// first color conflict found is a definitive "not bipartite," so this returns as soon as one
// appears rather than finishing the walk.
//
// A different property than Connectivity/ConnectedComponents.cs despite the superficial
// resemblance (both are multi-root walks over IGraphTopology): ConnectedComponents is defined
// by reachability, bipartiteness by 2-colorability - a graph can be one connected component and
// still fail bipartiteness, or be bipartite across many components. That's why this lives in
// its own Bipartiteness/ folder rather than joining Connectivity/ - "one file" alone isn't a
// reason to share a folder, since Ancestry/, Connectivity/, and Paths/ are already single-file
// folders in this repo.
//
// Two unenforced preconditions, the same "wrong answer, never an exception" shape
// TopologicalSort.cs's own header documents: (1) IGraphTopology.GetChildren has no
// directed/undirected distinction, so this assumes the caller's adjacency is symmetric - an
// asymmetric (directed-only) input can silently produce a meaningless bool, since a color
// conflict is only checked in the direction each edge happens to be walked; (2) nodes must
// include at least one representative per connected component, the same self-discovering
// shape ConnectedComponents.Count relies on (undocumented there, stated explicitly here) - a
// component with no representative in nodes is never visited or colored at all.
internal static class BipartiteCheck
{
    public static bool IsBipartite<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        IEnumerable<TNode> nodes)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var color = new Dictionary<TNode, bool>();

        foreach (var root in nodes)
        {
            if (color.ContainsKey(root))
            {
                continue;
            }

            if (!TryColorComponent<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(root, color))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryColorComponent<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode root, Dictionary<TNode, bool> color)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var frontier = new Queue<TNode>();
        color[root] = false;
        frontier.Enqueue(root);

        while (frontier.Count > 0)
        {
            var node = frontier.Dequeue();

            if (!TryColorChildren<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(node, color, frontier))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryColorChildren<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, Dictionary<TNode, bool> color, Queue<TNode> frontier)
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

            if (!color.TryGetValue(child, out var childColor))
            {
                color[child] = !color[node];
                frontier.Enqueue(child);
            }
            else if (childColor == color[node])
            {
                return false;
            }
        }

        return true;
    }
}
