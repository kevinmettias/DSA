using System.Text;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.CrackingTheSafe;

// LeetCode 753. Cracking the Safe: find a string of digits base k, length k^n + n - 1,
// containing every possible password of length n as a substring - a de Bruijn
// sequence, built by a greedy-with-undo search over the de Bruijn graph's edges. A
// candidate digit is only offered when appending it would extend the answer with a
// password not yet seen; choosing records that password before appending so undoing
// can look the same password back up (by re-reading the trailing N characters of the
// still-appended answer) and remove it again on backtrack.
//
// Both strategies do the identical choose/explore/unchoose walk and differ only in
// whether it is hand-specialized or composed from this repo's generic
// Backtrack.TrySearch engine (state mutated in place across recursive calls, not a
// fresh snapshot per node - the same shape Backtrack.cs/BacktrackingSteps.cs's own doc
// comments describe).
internal static class CrackingTheSafeSolution
{
    // The textbook answer: a hand-specialized recursive greedy-with-undo search over a
    // BCL HashSet<string> of passwords seen so far. Deliberately written without this
    // repo's generic engine - it is the arm the composed strategy below has to justify
    // itself against.
    public static string CrackSafeByGreedyRecursion(int n, int k)
    {
        var state = new SpecializedState(
            n, k, new StringBuilder(new string('0', n)), new HashSet<string> { new string('0', n) });

        Search(state);
        return state.Answer.ToString();
    }

    private static bool Search(SpecializedState state)
    {
        if (state.Visited.Count == state.Total)
        {
            return true;
        }

        var prefix = state.Answer.ToString(state.Answer.Length - (state.N - 1), state.N - 1);

        for (var digit = 0; digit < state.K; digit++)
        {
            var password = prefix + (char)('0' + digit);
            if (state.Visited.Add(password) && TryExtend(state, password, digit))
            {
                return true;
            }
        }

        return false;
    }

    // The "try password, recurse, undo on failure" branch from Search's digit loop:
    // appends the digit, recurses, and only rolls back the append/visited-add if the
    // recursive search did not find a full Eulerian path from here.
    private static bool TryExtend(SpecializedState state, string password, int digit)
    {
        state.Answer.Append((char)('0' + digit));

        if (Search(state))
        {
            return true;
        }

        state.Answer.Length -= 1;
        state.Visited.Remove(password);
        return false;
    }

    // This repo's generic Backtrack.TrySearch engine, closed over a shared mutable
    // state carrying the same StringBuilder answer plus a Set<string> of passwords
    // already covered that the specialized walk above carries as a BCL HashSet.
    public static string CrackSafeByBacktrackEngine(int n, int k)
    {
        var state = new EngineState(n, k);
        string? answer = null;

        var steps = new BacktrackingSteps<EngineState, int>(
            IsSolution: s => s.Visited.Count == s.Total,
            Candidates: CandidateDigits,
            Choose: ChooseDigit,
            Unchoose: UnchooseDigit,
            OnSolution: s =>
            {
                answer = s.Answer.ToString();
                return true;
            });

        Backtrack.TrySearch(state, steps);

        return answer!;
    }

    private static void ChooseDigit(EngineState state, int digit)
    {
        var nextPassword = NextPassword(state, digit);
        state.Visited.TryAdd(nextPassword);
        state.Answer.Append((char)('0' + digit));
    }

    private static void UnchooseDigit(EngineState state, int digit)
    {
        state.Visited.TryRemove(CurrentSuffix(state));
        state.Answer.Length -= 1;
    }

    private static IEnumerable<int> CandidateDigits(EngineState state)
    {
        for (var digit = 0; digit < state.K; digit++)
        {
            var nextPassword = NextPassword(state, digit);
            if (!state.Visited.Has(nextPassword))
            {
                yield return digit;
            }
        }
    }

    private static string NextPassword(EngineState state, int digit) =>
        Suffix(state, state.N - 1) + (char)('0' + digit);

    private static string CurrentSuffix(EngineState state) => Suffix(state, state.N);

    private static string Suffix(EngineState state, int length) =>
        state.Answer.ToString(state.Answer.Length - length, length);

    private sealed class EngineState
    {
        public EngineState(int n, int k)
        {
            N = n;
            K = k;
            Total = (int)Math.Pow(k, n);
            Answer = new StringBuilder(new string('0', n));
            Visited = new Set<string>();
            Visited.TryAdd(new string('0', n));
        }

        public int N { get; }
        public int K { get; }
        public int Total { get; }
        public StringBuilder Answer { get; }
        public Set<string> Visited { get; }
    }

    private sealed record SpecializedState(int N, int K, StringBuilder Answer, HashSet<string> Visited)
    {
        public int Total => (int)Math.Pow(K, N);
    }
}
