namespace DSAExperimentation.LeetCode.LongestBalancedSubarrayI;

// LeetCode 3719. Longest Balanced Subarray I: how long is the longest subarray
// whose distinct-even-value count equals its distinct-odd-value count.
//
// n <= 1500 keeps the intended O(n^2) expanding-window scan comfortably fast, so
// both strategies here share that complexity and differ only in which set
// tracks "already counted" - see LongestBalancedSubarrayII for the
// O(n log^2 n) segment-tree strategy this problem's harder sibling (n <= 1e5)
// actually forces.
internal static class LongestBalancedSubarrayISolution
{
    // The textbook answer: reset a fresh pair of hash sets at every start index
    // and extend the window rightward, counting each distinct value once.
    // Deliberately written without this repo's primitives - the arm the composed
    // strategy below has to justify itself against.
    public static int FindLongestBalancedLengthByBruteForce(int[] nums) =>
        LongestBalancedLength(nums, HashSetParityTally.Instance);

    // Same O(n^2) expanding-window shape, composed from this repo's own
    // Set<int> (TryAdd's newly-added bool is exactly HashSet.Add's return
    // contract) instead of a hand-rolled BCL HashSet pair.
    public static int FindLongestBalancedLengthByDistinctSetScan(int[] nums) =>
        LongestBalancedLength(nums, SetParityTally.Instance);

    // The scan both strategies run: every start index extends its own window
    // rightward, and the widest window whose two parity tallies are level is the
    // answer. Which structure holds a parity's distinct values is the arms' only
    // disagreement, so it arrives as a named strategy type - the decision it makes,
    // each input, and the contract both arms answer to are all written down there.
    private static int LongestBalancedLength<TSet>(int[] nums, IParityTally<TSet> tally)
    {
        var longest = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var window = LongestWindowFrom(nums, start, tally);
            longest = Math.Max(longest, window);
        }

        return longest;
    }

    // One start index's rightward extension: `sides` holds a tally per parity, evens
    // at index 0 and odds at index 1, so each value is rated into its own parity's
    // tally. A second sighting of a value adds nothing - that is what counting
    // *distinct* values means - so the window only counts while the totals are level.
    private static int LongestWindowFrom<TSet>(int[] nums, int start, IParityTally<TSet> tally)
    {
        var sides = new[] { tally.Fresh(), tally.Fresh() };
        var totals = new int[2];
        var longest = 0;

        for (var end = start; end < nums.Length; end++)
        {
            var value = nums[end];
            var isEven = value % 2 == 0;
            var parity = isEven ? 0 : 1;
            totals[parity] = tally.Count(sides[parity], value);

            if (totals[0] == totals[1])
            {
                longest = Math.Max(longest, end - start + 1);
            }
        }

        return longest;
    }
}
