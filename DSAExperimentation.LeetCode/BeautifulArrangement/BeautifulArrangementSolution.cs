using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.BeautifulArrangement;

// LeetCode 526. Beautiful Arrangement: count permutations of [1..n] where every
// 1-indexed position i holds a value v with v % i == 0 or i % v == 0.
//
// The two strategies differ in when the divisibility rule is checked: generate
// every full permutation and reject it afterward, or fold the rule directly into
// this repo's own Backtrack.Search Candidates step so an illegal value is never
// placed in the first place.
internal static class BeautifulArrangementSolution
{
    // The textbook answer: generate every full permutation of [1..n] and check it
    // only once it is complete - O(n!) permutations, each fully materialized
    // regardless of how early it violates the rule. Deliberately written without
    // this repo's primitives; the arm the pruned backtracking strategy below has
    // to justify itself against.
    public static int CountByGenerateThenFilter(int size)
    {
        var arrangement = new ArrangementState(size, new bool[size], new int[size]);

        return Generate(arrangement, 0);
    }

    // This repo's own Backtrack.Search engine, the same Permutations/
    // PermutationSequence composition, with the divisibility rule folded directly
    // into Candidates so an illegal value is never placed and the branch is
    // pruned immediately instead of discovered n steps later, once a full
    // permutation has already been built.
    public static int CountByPrunedBacktracking(int size)
    {
        var count = 0;
        var state = new State(size);

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == size,
            s =>
            {
                if (s.Values.Count == size)
                {
                    return [];
                }

                var position = s.Values.Count + 1;
                return Enumerable.Range(1, size).Where(v => !s.Used[v - 1] && (v % position == 0 || position % v == 0));
            },
            (s, v) => { s.Used[v - 1] = true; s.Values.Add(v); },
            (s, v) => { s.Used[v - 1] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            _ => count++);

        return count;
    }

    private static int Generate(ArrangementState arrangement, int depth)
    {
        if (depth == arrangement.N)
        {
            return IsBeautiful(arrangement) ? 1 : 0;
        }

        var count = 0;

        for (var v = 1; v <= arrangement.N; v++)
        {
            if (!arrangement.Used[v - 1])
            {
                count += PlaceAndRecurse(arrangement, depth, v);
            }
        }

        return count;
    }

    private static int PlaceAndRecurse(ArrangementState arrangement, int depth, int value)
    {
        arrangement.Used[value - 1] = true;
        arrangement.Values[depth] = value;
        var count = Generate(arrangement, depth + 1);
        arrangement.Used[value - 1] = false;

        return count;
    }

    private static bool IsBeautiful(ArrangementState arrangement)
    {
        for (var position = 1; position <= arrangement.N; position++)
        {
            var v = arrangement.Values[position - 1];
            if (v % position != 0 && position % v != 0)
            {
                return false;
            }
        }

        return true;
    }

    private readonly record struct ArrangementState(int N, bool[] Used, int[] Values);

    private sealed record State
    {
        public bool[] Used { get; }
        public List<int> Values { get; } = [];

        public State(int length) => Used = new bool[length];
    }
}
