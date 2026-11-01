namespace DSAExperimentation.Algorithms.Backtracking;

// An immutable value bundle grouping Search's five delegate parameters - the same
// "group related parameters into a type" recipe SortBounds/SearchRange already used
// (ARCHITECTURE.md §11.3), not a Representation or a closed-set Topology witness:
// each delegate is an open runtime object for the same reason DepthFirstSearch's
// successors is (§12.1) - the space of valid candidate/choose/unchoose logic is
// arbitrary caller code (a Sudoku digit-checker and a Word Search neighbor-filter
// share no enumerable "kind"), not a closed set this library could formalize.
//
// OnSolution returns bool, not void: true means "stop the whole search now" (Word
// Search/Sudoku's "find one and quit"), false means "keep enumerating" (Subsets/
// Permutations/N-Queens' count). Candidates is also where all pruning lives -
// Sudoku's dead-end detection, N-Queens' attack check - there is no separate
// "branch is dead" signal, the same way DepthFirstSearch has no signal beyond an
// empty successors set.
/// <summary>The five steps a backtracking search runs over one shared, mutable state.</summary>
/// <param name="IsSolution">Whether a state is a solution to report.</param>
/// <param name="Candidates">
/// The legal choices from a state, where all pruning happens; they must run out along every path.
/// </param>
/// <param name="Choose">Applies a choice to the state.</param>
/// <param name="Unchoose">Undoes a choice exactly, restoring everything IsSolution and Candidates read.</param>
/// <param name="OnSolution">
/// Receives each solution state and returns true to stop the whole search, false to go on.
/// </param>
public readonly record struct BacktrackingSteps<TState, TChoice>(
    Func<TState, bool> IsSolution,
    Func<TState, IEnumerable<TChoice>> Candidates,
    Action<TState, TChoice> Choose,
    Action<TState, TChoice> Unchoose,
    Func<TState, bool> OnSolution);
