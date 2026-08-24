using DSAExperimentation.Graph.Contracts.Ordering;

namespace DSAExperimentation.Graph.Contracts.Topologies;

// The general contract every walker, fold, and reduce strategy actually needs:
// given a node, what's adjacent to it. Nothing here requires acyclicity or unique
// ancestry - that's what ITreeTopology promises on top of this. Algorithms
// constrained on ITreeTopology get to skip visited-tracking/memoization because of
// that extra promise; algorithms constrained on this bare interface can't assume
// it and must defend against cycles/sharing themselves.
internal interface IGraphTopology<TNode, TChildren>
    where TNode : class
    where TChildren : struct, IChildren<TNode>
{
    static abstract TChildren GetChildren(TNode node);
}
