namespace DSAExperimentation.DataStructures.Graph.States;

/// <summary>One state a state graph keeps, and the kept states one move away.</summary>
/// <typeparam name="TState">One state of the space.</typeparam>
/// <param name="state">The state this node stands for.</param>
public sealed class StateNode<TState>(TState state)
{
    /// <summary>The state this node stands for.</summary>
    public TState State { get; } = state;

    /// <summary>The kept states one move away, in the order the space writes them, filled in while the graph is built.</summary>
    public List<StateNode<TState>> Successors { get; } = [];

    public override string ToString() => State?.ToString() ?? string.Empty;
}
