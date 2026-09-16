using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.MaximumProductOfTheLengthOfTwoPalindromicSubsequences;

// LeetCode 2002. Maximum Product of the Length of Two Palindromic Subsequences: pick two
// disjoint subsequences of text that both read as palindromes and maximize the product of
// their lengths. LeetCode caps text.Length at 12, so the intended solution genuinely is the
// 2^n subsequence space rather than a shortcut around a smarter algorithm.
//
// Both strategies enumerate that same space, keep every subsequence that is a palindrome
// as an index bitmask paired with its length, and finish with the identical pairwise scan
// for the best non-overlapping (mask & mask == 0) pair. What separates them is how the
// subsequences are generated:
//
//   BitmaskScan counts an int from 1 to 2^n - 1 and materializes each mask's index list -
//   the textbook arm, all BCL, first-class here rather than left as a benchmark-private
//   helper so that it is actually asserted.
//
//   Backtrack walks this repo's own Backtrack.Search choose/explore/unchoose recursion,
//   where every node of the decision tree is already a subsequence, so IsSolution is
//   unconditionally true and OnSolution just inspects the chosen indices - the same
//   exhaustive-enumeration shape SubsetsSolution uses, with the index list itself
//   available for the palindrome check instead of having to be rebuilt from a mask.
//
// A single-character input has no second disjoint palindrome to pair with, so the answer
// is 0 - both arms report it from the pairwise scan finding no pair at all.
internal static class MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution
{
    // The naive arm: enumerate every non-empty index bitmask directly.
    public static int MaxProductByBitmaskScan(string text)
    {
        var totalMasks = 1 << text.Length;
        var palindromes = new List<(int Mask, int Length)>();

        for (var mask = 1; mask < totalMasks; mask++)
        {
            if (!IsPalindromicMask(text, mask))
            {
                continue;
            }

            palindromes.Add((mask, BitCount(mask)));
        }

        return BestDisjointProduct(palindromes);
    }

    private static bool IsPalindromicMask(string text, int mask)
    {
        var indices = new List<int>();

        for (var i = 0; i < text.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                indices.Add(i);
            }
        }

        return IsPalindromicSubsequence(text, indices);
    }

    private static int BitCount(int mask)
    {
        var count = 0;

        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }

    // This repo's own choose/explore/unchoose walk over the same 2^n space.
    public static int MaxProductByBacktrack(string text)
    {
        var palindromes = new List<(int Mask, int Length)>();
        var state = new SubsequenceState();

        Backtrack.Search<SubsequenceState, int>(
            state,
            isSolution: static _ => true,
            candidates: st => Enumerable.Range(st.NextIndex, text.Length - st.NextIndex),
            choose: (st, index) =>
            {
                st.Indices.Add(index);
                st.NextIndex = index + 1;
            },
            unchoose: (st, _) => st.Indices.RemoveAt(st.Indices.Count - 1),
            onSolution: st => RecordIfPalindromic(text, st, palindromes));

        return BestDisjointProduct(palindromes);
    }

    private static void RecordIfPalindromic(
        string text, SubsequenceState state, List<(int Mask, int Length)> palindromes)
    {
        if (state.Indices.Count == 0 || !IsPalindromicSubsequence(text, state.Indices))
        {
            return;
        }

        var mask = 0;

        foreach (var index in state.Indices)
        {
            mask |= 1 << index;
        }

        palindromes.Add((mask, state.Indices.Count));
    }

    private static int BestDisjointProduct(List<(int Mask, int Length)> palindromes)
    {
        var best = 0;

        for (var i = 0; i < palindromes.Count; i++)
        {
            for (var j = i + 1; j < palindromes.Count; j++)
            {
                if ((palindromes[i].Mask & palindromes[j].Mask) == 0)
                {
                    best = Math.Max(best, palindromes[i].Length * palindromes[j].Length);
                }
            }
        }

        return best;
    }

    private static bool IsPalindromicSubsequence(string text, List<int> indices)
    {
        var left = 0;
        var right = indices.Count - 1;

        while (left < right)
        {
            if (text[indices[left]] != text[indices[right]])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }

    private sealed class SubsequenceState
    {
        public List<int> Indices { get; } = [];

        public int NextIndex { get; set; }
    }
}
