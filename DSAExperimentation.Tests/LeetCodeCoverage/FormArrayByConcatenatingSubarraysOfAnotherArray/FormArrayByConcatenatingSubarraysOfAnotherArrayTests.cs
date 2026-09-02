using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FormArrayByConcatenatingSubarraysOfAnotherArray;

// LeetCode 1764. Form Array by Concatenating Subarrays of Another Array: this
// repo's StringMatching primitives (PrefixFunctionSearch/ZFunction/RollingHashSearch)
// only operate over ReadOnlySpan<char>, so nums/groups' ints are first compressed
// into a shared char alphabet via this repo's own HashMap<int,char> - an injective
// mapping, since the at-most ~1,600 distinct values across nums and every group fit
// comfortably under char's 65,536-value range, so equal ints always get equal chars
// and distinct ints always get distinct chars. That turns "does group occur as a
// contiguous subarray of nums" into ordinary char pattern search, which
// PrefixFunctionSearch (KMP) already solves. Each group is searched only in the
// unconsumed suffix of nums, taking the first (leftmost) match - matching as early
// as possible never removes room a later group could have used, which is what makes
// this greedy left-to-right scan correct for the disjoint, in-order constraint.
public sealed partial class FormArrayByConcatenatingSubarraysOfAnotherArrayTests
{
    [Fact]
    public void CanChoose_GroupsMatchDisjointRunsInOrder_ReturnsTrue()
    {
        int[][] groups = [[1, -1, -1], [3, -2, 0]];
        int[] nums = [1, -1, 0, 1, -1, -1, 3, -2, 0];

        var actual = CanChoose(groups, nums);
        Assert.True(actual);
    }

    [Fact]
    public void CanChoose_FirstGroupNeverOccursInOrder_ReturnsFalse()
    {
        int[][] groups = [[10, -2], [1, 2, 3, 4]];
        int[] nums = [1, 2, 1, 1, 2, 2, -2, 10];

        var actual = CanChoose(groups, nums);
        Assert.False(actual);
    }

    [Fact]
    public void CanChoose_SecondGroupWouldOverlapFirstsGreedyMatch_ReturnsFalse()
    {
        int[][] groups = [[1, 2, 3], [3, 4]];
        int[] nums = [7, 7, 1, 2, 3, 4, 7, 7];

        var actual = CanChoose(groups, nums);
        Assert.False(actual);
    }

    private static bool CanChoose(int[][] groups, int[] nums)
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
