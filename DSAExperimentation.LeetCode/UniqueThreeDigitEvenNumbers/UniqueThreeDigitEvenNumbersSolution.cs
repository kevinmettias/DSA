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
        var chosen = new List<int>(Length);
        var distinct = new HashSet<int>();

        Search(digits, used, chosen, distinct);

        return distinct.Count;
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
                ? NoCandidates()
                : UnusedPositions(s, digits.Length),
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

    // No positions are left to choose once every slot has been filled.
    private static IEnumerable<int> NoCandidates() => [];

    // The digit positions still free to choose, in array order.
    private static IEnumerable<int> UnusedPositions(State s, int digitCount) =>
        Enumerable.Range(0, digitCount).Where(i => !s.Used[i]);

    // The choose/explore/unchoose step: three chosen digits are one candidate number,
    // otherwise every still-unused position is tried in turn and un-chosen on the way
    // back. chosen.Count is the slot being filled, so no separate depth is needed.
    private static void Search(int[] digits, bool[] used, List<int> chosen, HashSet<int> distinct)
    {
        if (chosen.Count == Length)
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
            chosen.Add(digits[i]);
            Search(digits, used, chosen, distinct);
            chosen.RemoveAt(chosen.Count - 1);
            used[i] = false;
        }
    }

    // Only numbers with a nonzero leading digit and an even last digit count.
    private static void RecordIfValid(List<int> chosen, HashSet<int> distinct)
    {
        if (chosen[0] == 0 || chosen[2] % 2 != 0)
        {
            return;
        }

        distinct.Add((chosen[0] * 10 + chosen[1]) * 10 + chosen[2]);
    }

    private sealed record State
    {
        public bool[] Used { get; }

        public List<int> Values { get; } = [];

        public State(int length) => Used = new bool[length];
    }
}
