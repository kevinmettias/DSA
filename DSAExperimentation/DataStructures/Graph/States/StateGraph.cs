using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.Graph.States;

// The explicit graph of a state space: every state the filter keeps becomes a node, joined to the
// kept states one move away. What a state is and how it moves is the space's business, usually a
// Domain model's (a combination lock, a sliding puzzle); this type only builds and looks up the
// graph, so each of those models stops carrying a node, a topology and a builder of its own.
/// <summary>Every kept state of a state space, each joined to the kept states one move away.</summary>
/// <typeparam name="TState">One state of the space.</typeparam>
public sealed class StateGraph<TState>
{
    private readonly HashMap<TState, StateNode<TState>> _nodesByState;

    /// <summary>How many states the graph kept.</summary>
    public int Count => _nodesByState.Count;

    private StateGraph(HashMap<TState, StateNode<TState>> nodesByState) => _nodesByState = nodesByState;

    // One pass makes a node of every kept state, a second joins each to its kept successors. The
    // successors of every state are written into one buffer, sized once from the space.
    /// <summary>Builds the graph of a space's states that a filter keeps.</summary>
    /// <typeparam name="TSpace">The space, as a struct so its calls are direct.</typeparam>
    /// <typeparam name="TFilter">The filter, as a struct so its calls are direct.</typeparam>
    /// <param name="space">The states and their moves.</param>
    /// <param name="filter">Which states become nodes.</param>
    /// <returns>The graph, each node joined to its kept successors in the order the space writes them.</returns>
    public static StateGraph<TState> Build<TSpace, TFilter>(TSpace space, TFilter filter)
        where TSpace : struct, IStateSpace<TState>
        where TFilter : struct, IStateFilter<TState>
    {
        var nodesByState = NodesOf(space, filter);
        var successors = new TState[space.MaxSuccessors];

        foreach (var node in nodesByState.Values)
        {
            var count = space.WriteSuccessors(node.State, successors);

            for (var i = 0; i < count; i++)
            {
                if (nodesByState.TryGetValue(successors[i], out var successor))
                {
                    node.Successors.Add(successor);
                }
            }
        }

        return new StateGraph<TState>(nodesByState);
    }

    private static HashMap<TState, StateNode<TState>> NodesOf<TSpace, TFilter>(TSpace space, TFilter filter)
        where TSpace : struct, IStateSpace<TState>
        where TFilter : struct, IStateFilter<TState>
    {
        var nodesByState = new HashMap<TState, StateNode<TState>>();

        for (var index = 0; index < space.StateCount; index++)
        {
            var state = space.StateAt(index);

            if (filter.IsKept(state))
            {
                nodesByState.Set(state, new StateNode<TState>(state));
            }
        }

        return nodesByState;
    }

    /// <summary>Looks up the node for a state.</summary>
    /// <param name="state">The state looked up.</param>
    /// <param name="node">The state's node when the graph kept it; otherwise unspecified.</param>
    /// <returns>True when the graph kept the state; false when the filter left it out or the space has no such state.</returns>
    public bool TryGetNode(TState state, out StateNode<TState> node) => _nodesByState.TryGetValue(state, out node);
}
