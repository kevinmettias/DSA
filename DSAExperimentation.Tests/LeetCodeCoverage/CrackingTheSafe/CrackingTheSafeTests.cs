using System.Text;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CrackingTheSafe;

// LeetCode 753. Cracking the Safe: this repo's generic Backtrack.TrySearch engine
// (choose/explore/unchoose over one shared mutable state), closed over a shared
// StringBuilder answer plus a Set<string> of passwords already covered - the same
// greedy-with-undo search Backtrack.cs/BacktrackingSteps.cs's own doc comments
// describe (state mutated in place across recursive calls, not a fresh snapshot
// per node), just walking a de Bruijn graph's edges instead of a Sudoku board's
// cells. A candidate digit is only offered when appending it would extend the
// answer with a password not yet seen; Choose records that password before
// appending so Unchoose can look the same password back up (by re-reading the
// trailing N characters of the still-appended answer) and remove it again on
// backtrack.
public sealed partial class CrackingTheSafeTests
{
    [Fact]
    public void CrackSafe_NEqualsOneKEqualsTwo_CoversBothOneDigitPasswords()
    {
        var safe = CrackSafe(n: 1, k: 2);

        AssertCoversEveryPassword(safe, n: 1, k: 2);
    }

    [Fact]
    public void CrackSafe_NEqualsTwoKEqualsTwo_CoversAllTwoDigitPasswords()
    {
        var safe = CrackSafe(n: 2, k: 2);

        AssertCoversEveryPassword(safe, n: 2, k: 2);
    }

    [Fact]
    public void CrackSafe_NEqualsTwoKEqualsThree_CoversAllTwoDigitBaseThreePasswords()
    {
        var safe = CrackSafe(n: 2, k: 3);

        AssertCoversEveryPassword(safe, n: 2, k: 3);
    }

    private static void AssertCoversEveryPassword(string safe, int n, int k)
    {
        var total = (int)Math.Pow(k, n);
        Assert.Equal(total + n - 1, safe.Length);

        for (var password = 0; password < total; password++)
        {
            var passwordDigits = ToBaseK(password, n, k);
            Assert.Contains(passwordDigits, safe);
        }
    }

    private static string ToBaseK(int value, int n, int k)
    {
        var digits = new char[n];
        for (var i = n - 1; i >= 0; i--)
        {
            digits[i] = (char)('0' + (value % k));
            value /= k;
        }

        return new string(digits);
    }

    private static string CrackSafe(int n, int k)
    {
        var state = new State(n, k);
        string? answer = null;

        var steps = BuildSteps(result => answer = result);
        Backtrack.TrySearch(state, steps);

        return answer!;
    }

    private static BacktrackingSteps<State, int> BuildSteps(Action<string> onSolutionFound) =>
        new(
            IsSolution: s => s.Visited.Count == s.Total,
            Candidates: CandidateDigits,
            Choose: ChooseDigit,
            Unchoose: UnchooseDigit,
            OnSolution: s =>
            {
                onSolutionFound(s.Answer.ToString());
                return true;
            });

    private static void ChooseDigit(State s, int digit)
    {
        var nextPassword = NextPassword(s, digit);
        s.Visited.TryAdd(nextPassword);
        s.Answer.Append((char)('0' + digit));
    }

    private static void UnchooseDigit(State s, int digit)
    {
        s.Visited.TryRemove(CurrentSuffix(s));
        s.Answer.Length -= 1;
    }

    private static IEnumerable<int> CandidateDigits(State state)
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

    private static string NextPassword(State state, int digit) => Suffix(state, state.N - 1) + (char)('0' + digit);

    private static string CurrentSuffix(State state) => Suffix(state, state.N);

    private static string Suffix(State state, int length) => state.Answer.ToString(state.Answer.Length - length, length);

    private sealed class State
    {
        public State(int n, int k)
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
}
