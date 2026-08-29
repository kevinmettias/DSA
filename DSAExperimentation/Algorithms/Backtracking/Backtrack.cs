namespace DSAExperimentation.Algorithms.Backtracking;

// The primitive DepthFirstSearch structurally cannot give: choose/explore/unchoose
// mutation over one shared TState, not a fresh immutable snapshot per node. Two
// independent reasons rule DepthFirstSearch out rather than just underperforming
// (ARCHITECTURE.md §12):
//
// 1. DepthFirstSearch's visited HashSet is dead weight here, not merely unneeded -
//    a decision tree reached by exactly one choice-sequence from the root per node
//    can never produce the cross-branch revisit visited-tracking exists to catch
//    (unlike an arbitrary graph's successors, which can transpose). TrySearch below
//    carries no visited-set at all, permanently, the same "no faster tier possible"
//    shape UnguardedVisit has over a promised topology, except here there is no
//    guard tier to begin with because there is nothing to guard against.
// 2. Forcing a shared mutable board through Func<TNode,IEnumerable<TNode>> is an
//    active correctness bug, not just a missed optimization: DepthFirstSearch's
//    order/visited both store the node reference, so every already-recorded entry
//    silently reflects the buffer's *final* state once the walk finishes, not its
//    state at time of visit. Real recursion (this repo's own DepthFirstWalk/
//    DepthFirstTraversal Enter/Exit engine) has the "subtree finished, time to
//    undo" event for free via the call stack unwinding; DepthFirstSearch's explicit
//    worklist stack has no equivalent signal.
//
// TState : class, enforced rather than documented: mutations made inside Choose/
// Unchoose only propagate back to the caller and across the recursive TrySearch
// calls if TState aliases, not copies (the same law ARCHITECTURE.md §11.2 states
// for IIndexedSequence's Set, except that one can only be documented since it's
// satisfied by third-party struct witnesses - here TState flows straight into the
// delegates with nothing in between, so the constraint can close the footgun at
// compile time instead).
//
// Precondition law, the same shape as DepthFirstSearch's "successors must produce a
// finite reachable set" (§12.3): Candidates must eventually stop offering legal
// choices along every path - typically by construction (state strictly shrinks the
// remaining choice set, or a bounded target/depth caps recursion), not checked here.
//
// Unchoose must be Choose's exact inverse - restore every field IsSolution/
// Candidates read, not just the ones convenient to undo. Getting this right is what
// makes it safe for Candidates to be a lazy iterator that re-reads TState per
// MoveNext, not just an eagerly-materialized one.
//
// Two entry points, deliberately different names rather than one bool-returning
// method overloaded with a void sibling: TrySearch's bool return is a real,
// caller-meaningful "did a solution stop the search" signal (the Try-prefixed,
// attempt-returns-outcome shape this repo already uses elsewhere), while Search is
// the void convenience overload for the exhaustive-enumeration/counting shape
// (Subsets, Permutations, N-Queens' solution count, Combination Sum, Generate
// Parentheses) - its onSolution never signals "stop," so callers here can't
// accidentally truncate their own search by returning the wrong bool.
internal static class Backtrack
{
    public static bool TrySearch<TState, TChoice>(TState state, BacktrackingSteps<TState, TChoice> steps)
        where TState : class
    {
        if (steps.IsSolution(state) && steps.OnSolution(state))
        {
            return true;
        }

        return TryEachCandidate(state, steps);
    }

    private static bool TryEachCandidate<TState, TChoice>(TState state, BacktrackingSteps<TState, TChoice> steps)
        where TState : class
    {
        foreach (var choice in steps.Candidates(state))
        {
            steps.Choose(state, choice);
            var stopped = TrySearch(state, steps);
            steps.Unchoose(state, choice);

            if (stopped)
            {
                return true;
            }
        }

        return false;
    }

    public static void Search<TState, TChoice>(
        TState state,
        Func<TState, bool> isSolution,
        Func<TState, IEnumerable<TChoice>> candidates,
        Action<TState, TChoice> choose,
        Action<TState, TChoice> unchoose,
        Action<TState> onSolution)
        where TState : class
        => TrySearch(state, new BacktrackingSteps<TState, TChoice>(
            isSolution,
            candidates,
            choose,
            unchoose,
            OnSolution: s => { onSolution(s); return false; }));
}
