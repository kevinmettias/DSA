using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.TallestBillboard;

// LeetCode 956. Tallest Billboard: split the rods into two disjoint piles of equal
// height and report that height, or 0 when no non-empty equal split exists. The state
// that matters is (index, diff) - how far along the rod list we are, and the running
// height difference between the two piles - and each rod is either skipped, added to
// the taller pile, or added to the shorter one. A state reached by one choice path is
// the same state reached by any other, which is the whole difference between the two
// strategies here:
//
//   UnmemoizedRecursion re-solves (index, diff) once per path that reaches it - 3^N
//   calls, the textbook recursion written with nothing but the call stack.
//
//   MemoizedDiff threads the identical recurrence through this repo's own
//   Memoizer<TState,TResult>, keying on the exact (index, diff) pair, so each state is
//   solved once - the same (index, runningState) shape TargetSum already uses.
internal static class TallestBillboardSolution
{
    // Sentinel for "this branch cannot end with the piles level". Halved so that adding
    // a rod's height to it can never wrap around into a competitive answer.
    private const int Unreachable = int.MinValue / 2;

    // The naive arm: the same skip/taller/shorter recurrence with no cache at all.
    public static int MaxHeightByUnmemoizedRecursion(int[] rods) => Solve(rods, 0, 0);

    // The same recurrence, memoized on (index, diff) through Memoizer.
    public static int MaxHeightByMemoizedDiff(int[] rods) =>
        Memoizer.Memoize<(int Index, int Diff), int>((0, 0), new HeightsOverRods(rods));

    // The recurrence, as a named type: the skip/taller/shorter search for the tallest
    // level split reachable from one (index, diff) state. The rod list it reads arrives
    // through the primary constructor and the memoized continuation through `rest`.
    private sealed class HeightsOverRods(int[] rods) : IRecurrence<(int Index, int Diff), int>
    {
        public int Replay(
            (int Index, int Diff) state, IRecurrence<(int Index, int Diff), int> rest)
        {
            if (state.Index == rods.Length)
            {
                return state.Diff == 0 ? 0 : Unreachable;
            }

            var rod = rods[state.Index];
            var skip = rest.Replay((state.Index + 1, state.Diff), rest);
            var addToTaller = rod + rest.Replay((state.Index + 1, state.Diff + rod), rest);
            var addToShorter = rest.Replay((state.Index + 1, state.Diff - rod), rest);
            var tallerOrShorter = Math.Max(addToTaller, addToShorter);

            return Math.Max(skip, tallerOrShorter);
        }
    }

    private static int Solve(int[] rods, int index, int diff)
    {
        if (index == rods.Length)
        {
            return diff == 0 ? 0 : Unreachable;
        }

        var rod = rods[index];
        var skip = Solve(rods, index + 1, diff);
        var addToTaller = rod + Solve(rods, index + 1, diff + rod);
        var addToShorter = Solve(rods, index + 1, diff - rod);
        var tallerOrShorter = Math.Max(addToTaller, addToShorter);

        return Math.Max(skip, tallerOrShorter);
    }
}
