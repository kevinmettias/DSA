using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumNumberOfWorkSessionsToFinishTheTasks;

// LeetCode 1986. Minimum Number of Work Sessions to Finish the Tasks: given task
// durations and a session budget, the fewest sessions that finish every task, where
// a session may hold any subset of tasks whose durations sum to at most sessionTime.
//
// Both strategies are the same bitmask recursion over "which tasks are still
// unscheduled" (one bit per task, tasks.Length <= 14): FeasibleSessionMasks says
// whether a subset fits in one session, and the recurrence picks any non-empty
// feasible submask of what is left as "the next session" and recurses on the
// remainder, keeping whichever split needs the fewest further sessions. Submask
// enumeration (`sub = (sub - 1) & remaining`) walks the candidate sessions, the
// same bit trick ParallelCoursesIISolution uses.
//
// They differ only in whether the recursion remembers the states it has already
// solved: many different session splits reach the same remaining set, so the
// unmemoized arm re-derives that state once per split that reaches it, while the
// memoized arm routes the same recurrence through this repo's own Memoizer keyed on
// the remaining-task mask, so each state is solved exactly once and every later
// split that reaches it is a cache hit.
internal static class MinimumNumberOfWorkSessionsToFinishTheTasksSolution
{
    // The textbook answer: the same recurrence with no cache at all, deliberately
    // BCL-only inside (§17.5) - it is the arm the memoized strategy below has to
    // justify itself against, and until this migration it lived only in the
    // benchmark, where nothing ever asserted it.
    public static int MinSessionsByUnmemoizedRecursion(int[] tasks, int sessionTime)
    {
        var sessions = FeasibleSessionMasks.Build(tasks, sessionTime);

        return MinSessionsByUnmemoizedRecursion(sessions);
    }

    public static int MinSessionsByUnmemoizedRecursion(FeasibleSessionMasks sessions) =>
        SessionsFor(sessions, sessions.FullMask);

    private static int SessionsFor(FeasibleSessionMasks sessions, int remaining)
    {
        if (remaining == 0)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var sub = remaining; sub > 0; sub = (sub - 1) & remaining)
        {
            if (sessions.FitsInOneSession(sub))
            {
                best = Math.Min(best, 1 + SessionsFor(sessions, remaining ^ sub));
            }
        }

        return best;
    }

    // This repo's own Memoizer, keyed on the remaining-task bitmask - the same shape
    // ParallelCoursesII and SmallestSufficientTeam use, an int bitmask as the memo
    // state rather than a bare counter.
    public static int MinSessionsByMemoizedBitmaskDp(int[] tasks, int sessionTime)
    {
        var sessions = FeasibleSessionMasks.Build(tasks, sessionTime);

        return MinSessionsByMemoizedBitmaskDp(sessions);
    }

    public static int MinSessionsByMemoizedBitmaskDp(FeasibleSessionMasks sessions) =>
        Memoizer.Memoize(sessions.FullMask, new SessionsFromRemainingTasks(sessions));

    // The recurrence, as a named type: nothing left to schedule costs no sessions, and
    // otherwise one session is spent on whichever feasible subset finishes the fewest
    // further sessions over what it leaves behind.
    private sealed class SessionsFromRemainingTasks(FeasibleSessionMasks sessions) : IRecurrence<int, int>
    {
        public int Replay(int remaining, IRecurrence<int, int> rest)
        {
            if (remaining == 0)
            {
                return 0;
            }

            return 1 + BestOverFeasibleSubsets(rest, remaining);
        }

        private int BestOverFeasibleSubsets(IRecurrence<int, int> rest, int remaining)
        {
            var best = int.MaxValue;

            for (var sub = remaining; sub > 0; sub = (sub - 1) & remaining)
            {
                if (sessions.FitsInOneSession(sub))
                {
                    var sessionsAfter = rest.Replay(remaining ^ sub, rest);
                    best = Math.Min(best, sessionsAfter);
                }
            }

            return best;
        }
    }
}
