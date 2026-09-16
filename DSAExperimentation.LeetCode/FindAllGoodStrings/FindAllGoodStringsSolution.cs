using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindAllGoodStrings;

// LeetCode 1397. Find All Good Strings: how many length-n strings lie in
// [s1, s2] lexicographically and do not contain evil as a substring, modulo 1e9+7.
internal static class FindAllGoodStringsSolution
{
    // The textbook answer: walk every candidate from s1 to s2 in lexicographic
    // order and ask the BCL whether it contains evil. Deliberately written without
    // this repo's primitives - it is the arm the composed solution below has to
    // justify itself against, and it pays the full 26^n cost when the bounds span
    // the alphabet.
    public static int CountGoodStringsByEnumeration(
        int stringLength, LowerBound s1, UpperBound s2, ForbiddenSubstring evil)
    {
        var current = s1.Text[..stringLength].ToCharArray();
        var count = 0L;

        // Stops when the odometer reaches s2: that candidate is tested, counted, and the total then returned.
        while (true)
        {
            var candidate = new string(current);

            if (candidate.IndexOf(evil.Text, StringComparison.Ordinal) < 0)
            {
                count = (count + 1) % ModularArithmetic.Modulo;
            }

            if (string.CompareOrdinal(candidate, s2.Text) >= 0)
            {
                return (int)count;
            }

            Increment(current);
        }
    }

    // The odometer step: the next string of the same length in lexicographic order.
    private static void Increment(char[] value)
    {
        for (var i = value.Length - 1; i >= 0; i--)
        {
            if (value[i] < 'z')
            {
                value[i]++;
                return;
            }

            value[i] = 'a';
        }
    }

    // KMP's own prefix/failure function (PrefixFunctionSearch.ComputeFailureFunction
    // over evil) turns "how much of evil does the string built so far already match"
    // into a small automaton state. Wiring that automaton through Memoizer's
    // memoized recursion over (position, automaton state, isTightToS1, isTightToS2)
    // is then a standard digit DP: it counts every string in [s1, s2] whose
    // automaton state never reaches evil.Length, without ever materializing a
    // candidate string.
    public static int CountGoodStringsByAutomatonDigitDp(
        int stringLength, LowerBound s1, UpperBound s2, ForbiddenSubstring evil)
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(evil.Text);
        var bounds = new GoodStringBounds(stringLength, s1.Text, s2.Text, evil.Text, failure);

        var result = Memoizer.Memoize<(int Position, int Matched, bool TightLow, bool TightHigh), long>(
            (0, 0, true, true), new GoodStringDigitWalk(bounds));

        return (int)result;
    }

    /// <summary>
    /// The recurrence, named: how many strings remain from a (position, automaton state,
    /// tight-to-s1, tight-to-s2) state - zero once the automaton has matched all of evil,
    /// one at the end of the string, and otherwise the sum over every allowed next letter.
    /// </summary>
    private sealed class GoodStringDigitWalk(GoodStringBounds bounds)
        : IRecurrence<(int Position, int Matched, bool TightLow, bool TightHigh), long>
    {
        /// <inheritdoc/>
        public long Replay(
            (int Position, int Matched, bool TightLow, bool TightHigh) state,
            IRecurrence<(int Position, int Matched, bool TightLow, bool TightHigh), long> rest)
        {
            if (state.Matched == bounds.Evil.Length)
            {
                return 0;
            }

            if (state.Position == bounds.N)
            {
                return 1;
            }

            var low = state.TightLow ? BoundCharAt(bounds.S1, state.Position) : 'a';
            var high = state.TightHigh ? BoundCharAt(bounds.S2, state.Position) : 'z';
            var walk = new GoodStringWalk(bounds.Evil, bounds.Failure, state, low, high, rest);

            return SumTransitions(walk, low, high);
        }
    }

    private static char BoundCharAt(string bound, int position) => bound[position];

    private static long SumTransitions(GoodStringWalk walk, char low, char high)
    {
        var total = 0L;

        for (var c = low; c <= high; c++)
        {
            var contribution = ComputeTransitionContribution(walk, c);

            if (contribution is not null)
            {
                total = (total + contribution.Value) % ModularArithmetic.Modulo;
            }
        }

        return total;
    }

    internal readonly record struct GoodStringBounds(int N, string S1, string S2, string Evil, int[] Failure);

    private static long? ComputeTransitionContribution(GoodStringWalk walk, char candidateLetter)
    {
        var matched = AdvanceAutomaton(walk.Evil, walk.Failure, walk.State.Matched, candidateLetter);
        if (matched == walk.Evil.Length)
        {
            return null;
        }

        var next = (
            walk.State.Position + 1,
            matched,
            walk.State.TightLow && candidateLetter == walk.Low,
            walk.State.TightHigh && candidateLetter == walk.High);
        return walk.Rest.Replay(next, walk.Rest);
    }

    internal readonly record struct GoodStringWalk(
        string Evil,
        int[] Failure,
        (int Position, int Matched, bool TightLow, bool TightHigh) State,
        char Low,
        char High,
        IRecurrence<(int Position, int Matched, bool TightLow, bool TightHigh), long> Rest);

    // The KMP fallback walk PrefixFunctionSearch.ComputeFailureFunction's own
    // Advance performs internally, rebuilt here from its public failure array -
    // the one piece of automaton-transition logic this problem needs that
    // ComputeFailureFunction doesn't expose as a standalone step.
    private static int AdvanceAutomaton(string evil, int[] failure, int matched, char next)
    {
        while (matched > 0 && evil[matched] != next)
        {
            matched = failure[matched - 1];
        }

        var extendsMatch = evil[matched] == next;

        return extendsMatch ? MatchLengthWithNext(matched) : matched;
    }

    private static int MatchLengthWithNext(int matched) => matched + 1;

    // LC 1397's three operands, named for the roles they play here rather than left as
    // three adjacent `string` positions a caller could hand over the wrong way round
    // with the compiler none the wiser. `s1` and `s2` are the inclusive lexicographic
    // range the count is taken over and `evil` the substring no counted string may
    // contain - three different meanings, and s1 and s2 are not even symmetric with
    // each other.
    internal readonly record struct LowerBound(string Text);

    internal readonly record struct UpperBound(string Text);

    internal readonly record struct ForbiddenSubstring(string Text);
}
