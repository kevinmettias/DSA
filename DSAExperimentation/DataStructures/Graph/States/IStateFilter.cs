namespace DSAExperimentation.DataStructures.Graph.States;

// Which states of a space a graph keeps: a lock's dead combinations, a maze's walls. A struct, so
// StateGraph.Build asks IsKept once per state with a direct call.
/// <summary>Which states of a space a state graph keeps.</summary>
/// <typeparam name="TState">One state of the space.</typeparam>
public interface IStateFilter<TState>
{
    /// <summary>Tells whether the graph keeps a state.</summary>
    /// <param name="state">The state asked about.</param>
    /// <returns>True when the state becomes a node; false when the graph leaves it out.</returns>
    bool IsKept(TState state);
}
