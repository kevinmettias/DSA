namespace DSAExperimentation.Graph;

// Orthogonal to visit-timing: this only decides the sequence a node's own children
// are walked in. A node's *structural* order comes from the topology itself
// (GetChildren yields them in whatever order the node type defines - a named
// Left/Middle/Right field, or a list); IChildOrder is an optional override for
// walking that same structure differently (reverse, priority-sorted) without
// touching the topology.
public interface IChildOrder<TNode, TChildren, TOrderedChildren>
    where TChildren : struct, IChildren<TNode>
    where TOrderedChildren : struct, IChildren<TNode>
{
    static abstract TOrderedChildren Apply(TChildren children);
}
