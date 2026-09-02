using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.LongestConsecutiveSequence;

// LeetCode 128. Longest Consecutive Sequence: length of the longest run of
// consecutive integers present in an unsorted array, in O(n).
//
// A value only starts walking a run if its predecessor is absent from the
// set, so every element is touched by at most two operations total (one
// membership check as a non-starter, one linear walk as a starter) - the
// nested loop below stays O(n) despite looking quadratic.
internal static class LongestConsecutiveSequenceSolution
{
    public static int LongestConsecutiveBySetRunExpansion(int[] nums)
    {
        var present = new Set<int>(nums);
        var longest = 0;

        foreach (var value in nums)
        {
            if (present.Has(value - 1))
            {
                continue;
            }

            var current = value;

            while (present.Has(current))
            {
                current++;
            }

            longest = Math.Max(longest, current - value);
        }

        return longest;
    }
}
