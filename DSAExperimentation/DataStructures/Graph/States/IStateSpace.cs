namespace DSAExperimentation.DataStructures.Graph.States;

// A finite set of states and the moves between them, as a struct so StateGraph.Build's calls on it
// are direct and inline: a combination lock's wheel turns, a sliding puzzle's slides, a car's
// accelerate-and-reverse. The space numbers its states so the graph can enumerate them, and writes
// a state's successors into a buffer the builder owns, so no move allocates a list.
/// <summary>A finite set of states, numbered, and the moves that lead from one state to others.</summary>
/// <typeparam name="TState">One state of the space.</typeparam>
public interface IStateSpace<TState>
{
    /// <summary>How many states the space holds.</summary>
    int StateCount { get; }

    /// <summary>The most successors any one state has, which sizes the buffer moves are written into.</summary>
    int MaxSuccessors { get; }

    /// <summary>Names one state by its number.</summary>
    /// <param name="index">The state's number, in [0, <see cref="StateCount"/>).</param>
    /// <returns>The state numbered <paramref name="index"/>.</returns>
    TState StateAt(int index);

    /// <summary>Writes the states one move away from a state.</summary>
    /// <param name="state">The state moved from.</param>
    /// <param name="successors">Where to write them, at least <see cref="MaxSuccessors"/> long.</param>
    /// <returns>How many successors were written, from the start of <paramref name="successors"/>.</returns>
    int WriteSuccessors(TState state, Span<TState> successors);
}
