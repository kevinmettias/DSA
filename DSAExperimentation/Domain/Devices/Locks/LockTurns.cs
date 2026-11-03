using DSAExperimentation.DataStructures.Alphabets;
using DSAExperimentation.DataStructures.Graph.States;

namespace DSAExperimentation.Domain.Devices.Locks;

// A combination lock as a state space, for StateGraph: its combinations are the states, and a move
// clicks one wheel one symbol either way.
/// <summary>A combination lock's combinations as a state space, each one click of one wheel from the next.</summary>
/// <typeparam name="TSpec">The lock's build: its wheels and their symbols.</typeparam>
/// <typeparam name="TWheel">The alphabet each wheel carries.</typeparam>
public readonly struct LockTurns<TSpec, TWheel> : IStateSpace<string>
    where TSpec : struct, ILockSpec<TWheel>
    where TWheel : struct, IAlphabet
{
    /// <summary>Every combination of the lock.</summary>
    public int StateCount => CombinationLock<TSpec, TWheel>.CombinationCount;

    /// <summary>Two turns per wheel, one up and one down.</summary>
    public int MaxSuccessors => CombinationLock<TSpec, TWheel>.OneTurnAwayCount;

    /// <summary>Spells a combination by its number.</summary>
    /// <param name="index">The combination's number.</param>
    /// <returns>The combination, one symbol per wheel.</returns>
    public string StateAt(int index) => CombinationLock<TSpec, TWheel>.Combination(index);

    /// <summary>Writes the combinations one click of one wheel away.</summary>
    /// <param name="state">The combination turned from.</param>
    /// <param name="successors">Where to write them.</param>
    /// <returns>How many were written, two per wheel.</returns>
    public int WriteSuccessors(string state, Span<string> successors) => CombinationLock<TSpec, TWheel>.WriteOneTurnAway(state, successors);
}
