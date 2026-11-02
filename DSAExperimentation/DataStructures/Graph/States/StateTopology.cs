using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.DataStructures.Graph.States;

// A state graph is a general graph, not a DAG: most moves can be undone, so cycles are everywhere.
/// <summary>The graph topology over state nodes: a node's children are its successors.</summary>
/// <typeparam name="TState">One state of the space.</typeparam>
public readonly struct StateTopology<TState> : IGraphTopology<StateNode<TState>, ListChildren<StateNode<TState>>>
{
    /// <summary>Returns a node's successors as its children.</summary>
    /// <param name="node">The node whose children to return.</param>
    /// <returns>A view of the node's successors, in the order the space wrote them.</returns>
    public static ListChildren<StateNode<TState>> GetChildren(StateNode<TState> node) => new(node.Successors);
}
