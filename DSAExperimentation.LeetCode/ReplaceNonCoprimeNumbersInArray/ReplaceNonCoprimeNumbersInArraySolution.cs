using RepoLongStack = DSAExperimentation.DataStructures.Stack.Stack<long>;

namespace DSAExperimentation.LeetCode.ReplaceNonCoprimeNumbersInArray;

// LeetCode 2197. Replace Non-Coprime Numbers in Array: repeatedly replace any two
// adjacent numbers that share a factor with their LCM, until no adjacent pair does.
// The problem guarantees the final array is the same whatever order the merges
// happen in, which is what lets the two strategies below disagree about order and
// still agree about the answer.
//
// The interesting part is that a merge can create a NEW mergeable pair behind it:
// after [2, 3, 6] becomes [2, 6] the pair that now shares a factor is the one to
// the LEFT of where the work just happened. The two strategies are the two ways of
// coping with that - rescan the whole list from the front after every merge, or
// keep the finalized prefix on a stack whose top is always the only element a new
// number can possibly merge with.
internal static class ReplaceNonCoprimeNumbersInArraySolution
{
    // Textbook baseline: a BCL List<long>, scanned from the front for the first
    // mergeable adjacent pair and restarted from scratch after every single merge,
    // because a merge can cascade arbitrarily far back. Deliberately written
    // without this repo's primitives - it is the arm the stack strategy below has
    // to justify itself against.
    public static int[] ReplaceByRepeatedRescan(int[] nums)
    {
        var list = new List<long>(nums.Length);

        foreach (var num in nums)
        {
            list.Add(num);
        }

        var mergedAny = true;

        while (mergedAny)
        {
            mergedAny = MergeFirstNonCoprimePair(list);
        }

        return ToIntArray(list);
    }

    private static bool MergeFirstNonCoprimePair(List<long> list)
    {
        for (var i = 0; i < list.Count - 1; i++)
        {
            var gcd = Gcd(list[i], list[i + 1]);

            if (gcd > 1)
            {
                list[i] = list[i] / gcd * list[i + 1];
                list.RemoveAt(i + 1);
                return true;
            }
        }

        return false;
    }

    // This repo's own Stack<long> (the DailyTemperatures/NextGreaterElement
    // precedent) holding the merged prefix so far. Each new number keeps merging
    // into the top while it shares a factor with it, which cascades backward
    // exactly as far as needed and never rescans, because the stack's top is
    // always the most recently finalized element - the only one a new number can
    // be adjacent to.
    public static int[] ReplaceByStackCascade(int[] nums)
    {
        var stack = new RepoLongStack();

        foreach (var num in nums)
        {
            long current = num;

            while (stack.TryPeek(out var top) && Gcd(top, current) > 1)
            {
                stack.TryPop(out _);
                current = current / Gcd(top, current) * top;
            }

            stack.Push(current);
        }

        return DrainToArray(stack);
    }

    // The stack holds the answer bottom-up, so popping yields it reversed.
    private static int[] DrainToArray(RepoLongStack stack)
    {
        var values = new List<long>(stack.Count);

        while (stack.TryPop(out var value))
        {
            values.Add(value);
        }

        values.Reverse();
        return ToIntArray(values);
    }

    // Merging is LCM, so intermediate values are widened to long; LeetCode
    // guarantees every value in the final array fits an int.
    private static int[] ToIntArray(List<long> values)
    {
        var result = new int[values.Count];

        for (var i = 0; i < values.Count; i++)
        {
            result[i] = (int)values[i];
        }

        return result;
    }

    private static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);
}
