using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.UniqueThreeDigitEvenNumbers;

// LeetCode 3483. Unique 3-Digit Even Numbers: count the distinct 3-digit even
// numbers formable from digits, using each array position at most once and never
// leading with 0.
//
// Both strategies enumerate every ordered choice of 3 distinct positions - the
// textbook plain recursion over a used[] flag array, or this repo's own
// Backtrack.Search engine driving the identical choose/explore/unchoose shape (the
// same contrast PermutationsSolution draws between PermuteBySpecializedRecursion
// and PermuteByBacktracking) - each deduping the resulting numeric value
// differently, since repeated digit VALUES at different array positions can reach
// the same 3-digit number more than once (LC's own [0,2,2] example: 202 is
// reachable via two different index orderings).
internal static class UniqueThreeDigitEvenNumbersSolution
{
    private const int Length = 3;

    // Plain recursion over a BCL used[] flag array and a BCL HashSet<int> for
    // dedup - deliberately without this repo's backtracking engine or Set. The arm
    // CountByBacktracking has to justify itself against.
    public static int CountByIndexPermutationScan(int[] digits)
    {
        var used = new bool[digits.Length];
        var chosen = new int[Length];
        var distinct = new HashSet<int>();

        void Search(int depth)
        {
            if (depth == Length)
            {
                RecordIfValid(chosen, distinct);
                return;
            }

            for (var i = 0; i < digits.Length; i++)
            {
                if (used[i])
                {
                    continue;
                }

                used[i] = true;
                chosen[depth] = digits[i];
                Search(depth + 1);
                used[i] = false;
            }
        }

        Search(0);
        return distinct.Count;
    }

    private static void RecordIfValid(int[] chosen, HashSet<int> distinct)
    {
        if (chosen[0] == 0 || chosen[2] % 2 != 0)
        {
            return;
        }

        distinct.Add((chosen[0] * 10 + chosen[1]) * 10 + chosen[2]);
    }

    // This repo's Backtrack.Search primitive drives the identical
    // choose/explore/unchoose shape over chosen index positions, capped at Length
    // via IsSolution/Candidates the same way PermuteByBacktracking caps at
    // nums.Length; dedup goes through this repo's own Set<int> instead of a BCL
    // HashSet.
    public static int CountByBacktracking(int[] digits)
    {
        var state = new State(digits.Length);
        var distinct = new Set<int>();

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == Length,
            s => s.Values.Count == Length
                ? []
                : Enumerable.Range(0, digits.Length).Where(i => !s.Used[i]),
            (s, i) => { s.Used[i] = true; s.Values.Add(digits[i]); },
            (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            s =>
            {
                if (s.Values[0] != 0 && s.Values[2] % 2 == 0)
                {
                    distinct.TryAdd((s.Values[0] * 10 + s.Values[1]) * 10 + s.Values[2]);
                }
            });

        return distinct.Count;
    }

    private sealed class State(int length)
    {
        public bool[] Used { get; } = new bool[length];

        public List<int> Values { get; } = [];
    }
}
