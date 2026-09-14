using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FormArrayByConcatenatingSubarraysOfAnotherArray;

// LeetCode 1764. Form Array by Concatenating Subarrays of Another Array: can
// every group be found as a contiguous subarray of nums, with the chosen
// occurrences disjoint and in the groups' own order?
//
// Both strategies are the same greedy left-to-right scan - each group is searched
// only in the unconsumed suffix of nums and the first (leftmost) occurrence is
// taken, which is correct here because matching as early as possible never removes
// room a later group could have used. They differ only in how that one search is
// performed: a from-scratch re-comparison at every start position, or KMP.
internal static class FormArrayByConcatenatingSubarraysOfAnotherArraySolution
{
    // The textbook answer: for each group, walk every start position in the
    // unconsumed suffix and compare element by element, restarting the comparison
    // from the pattern's first element on any mismatch. O(n*m) per group.
    // Deliberately written with BCL arrays and index arithmetic only - it is the
    // arm the KMP strategy below has to justify itself against.
    public static bool CanChooseByNaiveSubarrayScan(int[][] groups, int[] nums)
    {
        var position = 0;

        foreach (var group in groups)
        {
            if (!TryFindSubarray(nums, group, position, out var match))
            {
                return false;
            }

            position = match + group.Length;
        }

        return true;
    }

    // The first start position at or after `from` where group occurs in nums.
    private static bool TryFindSubarray(int[] nums, int[] group, int from, out int match)
    {
        for (var start = from; start + group.Length <= nums.Length; start++)
        {
            if (MatchesAt(nums, group, start))
            {
                match = start;
                return true;
            }
        }

        match = 0;
        return false;
    }

    private static bool MatchesAt(int[] nums, int[] group, int start)
    {
        for (var i = 0; i < group.Length; i++)
        {
            if (nums[start + i] != group[i])
            {
                return false;
            }
        }

        return true;
    }

    // This repo's StringMatching primitives operate over ReadOnlySpan<char>, so
    // nums and every group are first compressed into a shared char alphabet via
    // this repo's own HashMap<int,char>. The mapping is injective - the at-most
    // ~1,600 distinct values across nums and the groups fit comfortably under
    // char's 65,536-value range - so equal ints always get equal chars and
    // distinct ints always get distinct chars. That turns "does this group occur
    // as a contiguous subarray" into ordinary char pattern search, which
    // PrefixFunctionSearch (KMP) already solves in O(n+m) guaranteed.
    public static bool CanChooseByCharCompressedKmpSearch(int[][] groups, int[] nums)
    {
        var codes = BuildCharCodes(groups, nums);
        var text = Encode(nums, codes);
        var position = 0;

        foreach (var group in groups)
        {
            var pattern = Encode(group, codes);
            var matches = PrefixFunctionSearch.FindAll(text.AsSpan(position), pattern);

            if (matches.Count == 0)
            {
                return false;
            }

            position += matches[0] + group.Length;
        }

        return true;
    }

    private static HashMap<int, char> BuildCharCodes(int[][] groups, int[] nums)
    {
        var codes = new HashMap<int, char>();
        AssignCodes(nums, codes);

        foreach (var group in groups)
        {
            AssignCodes(group, codes);
        }

        return codes;
    }

    // The code handed out is the map's current size, so the values are dense and
    // distinct - the only two properties the search needs of them.
    private static void AssignCodes(int[] values, HashMap<int, char> codes)
    {
        foreach (var value in values)
        {
            if (!codes.HasKey(value))
            {
                codes.Set(value, (char)codes.Count);
            }
        }
    }

    private static char[] Encode(int[] values, HashMap<int, char> codes)
    {
        var encoded = new char[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            codes.TryGetValue(values[i], out encoded[i]);
        }

        return encoded;
    }
}
