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
internal readonly record struct BacktrackingSteps<TState, TChoice>(
    Func<TState, bool> IsSolution,
    Func<TState, IEnumerable<TChoice>> Candidates,
    Action<TState, TChoice> Choose,
    Action<TState, TChoice> Unchoose,
    Func<TState, bool> OnSolution);
