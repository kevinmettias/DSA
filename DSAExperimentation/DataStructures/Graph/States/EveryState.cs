namespace DSAExperimentation.DataStructures.Graph.States;

/// <summary>The filter that keeps every state of a space.</summary>
/// <typeparam name="TState">One state of the space.</typeparam>
public readonly struct EveryState<TState> : IStateFilter<TState>
{
    public bool IsKept(TState state) => true;
}
