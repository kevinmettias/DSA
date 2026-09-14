using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

// LeetCode 2030. Smallest K-Length Subsequence With Occurrences of a Letter: the
// lexicographically smallest length-k subsequence of s that contains at least
// `repetition` copies of `letter`.
//
// Both strategies answer the same question and differ only in how they find each
// output character. The feasibility rules are identical either way: a character may
// only be taken while enough of the string remains to reach length k, and taking (or
// dropping) a `letter` occurrence must never leave fewer than `repetition` of them
// reachable - which is why both arms precompute the same suffix count of `letter`.
internal static class SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution
{
    // Marks "no admissible position found yet" while scanning a window.
    private const int NoPosition = -1;

    // The textbook answer: choose the output one slot at a time, and for each slot
    // rescan the whole still-feasible window from scratch for the smallest character
    // that leaves the remaining requirements satisfiable. Deliberately written
    // without this repo's primitives - plain arrays and an index cursor - because it
    // is the arm the monotonic-stack sweep below has to justify itself against.
    // O(n) per slot, so O(n*k) overall.
    public static string SmallestSubsequenceByWindowRescan(string s, int k, char letter, int repetition)
    {
        var context = new RescanContext(s, k, letter, repetition, BuildLetterSuffixCount(s, letter));
        var answer = new char[k];
        var progress = new ScanProgress();

        for (var slot = 0; slot < k; slot++)
        {
            SelectNextCharacter(context, answer, slot, progress);
        }

        return new string(answer);
    }

    private static void SelectNextCharacter(RescanContext context, char[] answer, int slot, ScanProgress progress)
    {
        var charsNeeded = context.K - slot;
        var lettersStillNeeded = Math.Max(0, context.Repetition - progress.LettersUsed);
        var windowEnd = context.S.Length - charsNeeded;

        var window = new WindowScan(lettersStillNeeded, charsNeeded - 1, progress.Cursor, windowEnd);
        var bestPosition = FindBestPosition(context, window);

        answer[slot] = context.S[bestPosition];
        if (context.S[bestPosition] == context.Letter)
        {
            progress.LettersUsed++;
        }

        progress.Cursor = bestPosition + 1;
    }

    // The smallest character in [Cursor, WindowEnd] that may still be taken.
    private static int FindBestPosition(RescanContext context, WindowScan window)
    {
        var bestPosition = NoPosition;

        for (var p = window.Cursor; p <= window.WindowEnd; p++)
        {
            if (!CanTakeWithoutStarvingLetter(context, window, p))
            {
                continue;
            }

            if (bestPosition == NoPosition || context.S[p] < context.S[bestPosition])
            {
                bestPosition = p;
            }
        }

        return bestPosition;
    }

    // Whether taking s[position] still leaves the outstanding `letter` requirement
    // satisfiable. It takes TWO counts, and dropping either one silently produces an
    // answer with too few `letter` occurrences: enough of them must remain in the
    // suffix, AND enough output slots must remain to hold them.
    private static bool CanTakeWithoutStarvingLetter(RescanContext context, WindowScan window, int position)
    {
        var takesLetter = context.S[position] == context.Letter;
        var stillOwed = Math.Max(0, window.LettersStillNeeded - (takesLetter ? 1 : 0));

        return context.LetterSuffixCount[position + 1] >= stillOwed && window.SlotsAfterPick >= stillOwed;
    }

    // One slot's worth of scanning state: how many `letter` occurrences the answer
    // still owes, how many slots are left after this one is filled, and the index
    // range still admissible.
    private readonly record struct WindowScan(
        int LettersStillNeeded, int SlotsAfterPick, int Cursor, int WindowEnd);

    // How far the rescan has committed: the first index still available, and how many
    // `letter` occurrences the answer already holds.
    private sealed class ScanProgress
    {
        public int Cursor { get; set; }

        public int LettersUsed { get; set; }
    }

    private readonly record struct RescanContext(
        string S, int K, char Letter, int Repetition, int[] LetterSuffixCount);

    // The same greedy monotonic-stack shape RemoveDuplicateLetters uses, built on this
    // repo's own Stack<char>, but popping is gated on two extra counts beyond "is the
    // top bigger" - enough characters must still remain ahead to reach length k, and
    // popping a `letter` occurrence must not drop the remaining supply below
    // `repetition`. Each character is pushed once and popped at most once, so the
    // whole sweep is O(n) rather than the baseline's O(n*k).
    public static string SmallestSubsequenceByMonotonicStack(string s, int k, char letter, int repetition)
    {
        var n = s.Length;
        var context = new SweepContext(s, n, k, letter, repetition, BuildLetterSuffixCount(s, letter));

        var stack = new RepoCharStack();
        var lettersInStack = 0;

        for (var i = 0; i < n; i++)
        {
            AdmitCharacter(context, stack, ref lettersInStack, i);
        }

        return DrainStack(stack);
    }

    private static void AdmitCharacter(SweepContext context, RepoCharStack stack, ref int lettersInStack, int index)
    {
        ShrinkWhileNotFeasible(context, stack, ref lettersInStack, index);

        if (stack.Count >= context.K)
        {
            return;
        }

        TryPushCharacter(context, stack, ref lettersInStack, context.S[index]);
    }

    private static void ShrinkWhileNotFeasible(SweepContext context, RepoCharStack stack, ref int lettersInStack, int index)
    {
        while (ShouldDropTop(context, stack, lettersInStack, index))
        {
            stack.TryPop(out var dropped);
            if (dropped == context.Letter)
            {
                lettersInStack--;
            }
        }
    }

    // The stack top is worth dropping when the character now arriving is smaller than
    // it AND the answer can still be completed without it: enough characters must
    // remain ahead to refill the stack to length k, and dropping the top must not
    // starve the `letter` quota.
    private static bool ShouldDropTop(
        SweepContext context, RepoCharStack stack, int lettersInStack, int index)
    {
        if (!stack.TryPeek(out var top) || top <= context.S[index])
        {
            return false;
        }

        return stack.Count + (context.N - index) > context.K
            && CanDropWithoutStarvingLetter(context, top, lettersInStack, index);
    }

    // Dropping `top` must not put the `letter` requirement out of reach: either it is
    // not a `letter` occurrence at all, or enough of them remain between the shortened
    // stack and the part of the string still to be swept.
    private static bool CanDropWithoutStarvingLetter(SweepContext context, char top, int lettersInStack, int index)
    {
        if (top != context.Letter)
        {
            return true;
        }

        return lettersInStack - 1 + context.LetterSuffixCount[index] >= context.Repetition;
    }

    private static void TryPushCharacter(
        SweepContext context, RepoCharStack stack, ref int lettersInStack, char candidate)
    {
        if (candidate == context.Letter)
        {
            stack.Push(candidate);
            lettersInStack++;
        }
        else if (context.K - stack.Count > context.Repetition - lettersInStack)
        {
            stack.Push(candidate);
        }
    }

    // Stack<char> hands back the most recently pushed character first, so the answer
    // is filled from its last slot backwards.
    private static string DrainStack(RepoCharStack stack)
    {
        var result = new char[stack.Count];
        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return new string(result);
    }

    private readonly record struct SweepContext(
        string S, int N, int K, char Letter, int Repetition, int[] LetterSuffixCount);

    // LetterSuffixCount[i] is how many `letter` occurrences sit at or after index i,
    // so LetterSuffixCount[n] is 0 and a feasibility check is a single lookup.
    private static int[] BuildLetterSuffixCount(string text, char letter)
    {
        var letterSuffixCount = new int[text.Length + 1];
        for (var i = text.Length - 1; i >= 0; i--)
        {
            var isLetter = text[i] == letter;
            letterSuffixCount[i] = letterSuffixCount[i + 1] + (isLetter ? 1 : 0);
        }

        return letterSuffixCount;
    }
}
