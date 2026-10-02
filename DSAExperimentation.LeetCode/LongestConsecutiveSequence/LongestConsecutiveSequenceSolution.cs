using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.LongestConsecutiveSequence;

// LeetCode 128. Longest Consecutive Sequence: length of the longest run of
// consecutive integers present in an unsorted array.
//
// Two strategies for the same runs: expand from a value whose predecessor is
// absent, backed by this repo's Set, or sort a copy and scan the ordered values
// once. The set arm is expected O(n) but hashes every value; the sort arm is
// O(n log n) and copies the array, but does no hashing. Which wins is what the
// pair exists to measure.
internal static class LongestConsecutiveSequenceSolution
{
    // Sorts a copy of the array and walks it once: after ordering, a run is
    // exactly a maximal stretch where each value is its predecessor plus one, and
    // duplicates are skipped. It pays an O(n log n) sort and a full copy against
    // the set arm's expected O(n), which is the trade the pair measures.
    public static int LongestConsecutiveBySortedScan(int[] nums)
    {
        if (nums.Length == 0)
        {
            return 0;
        }

        var ordered = (int[])nums.Clone();
        Array.Sort(ordered);

        var longest = 1;
        var current = 1;

        for (var i = 1; i < ordered.Length; i++)
        {
            if (ordered[i] == ordered[i - 1])
            {
                continue;
            }

            current = ordered[i] == ordered[i - 1] + 1 ? current + 1 : 1;
            longest = Math.Max(longest, current);
        }

        return longest;
    }

    // The set-backed arm: a value only starts walking a run if its predecessor is
    // absent from the set, so every element is touched by at most two operations
    // total (one membership check as a non-starter, one linear walk as a starter)
    // - the nested loop stays O(n) expected despite looking quadratic.
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
