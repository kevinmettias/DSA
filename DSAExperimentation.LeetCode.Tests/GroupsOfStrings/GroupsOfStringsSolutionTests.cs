using DSAExperimentation.LeetCode.GroupsOfStrings;

namespace DSAExperimentation.LeetCode.Tests.GroupsOfStrings;

// Harness only: both strategies live in GroupsOfStringsSolution, including the
// pairwise popcount baseline the pre-migration benchmark kept to itself - and
// which only counted groups there, never reporting the largest one LeetCode also
// asks for. The letter-set masks both strategies are handed are asserted on their own.
public sealed partial class GroupsOfStringsSolutionTests
{
    // (words, [number of groups, size of the largest group])
    public static TheoryData<string[], int[]> Examples =>
        new()
        {
            // "a"/"b" connect by REPLACE, "ab" by ADD; "cde" reaches none of them.
            { ["a", "b", "ab", "cde"], [2, 3] },

            // A pure add/delete chain, so the group only forms transitively.
            { ["a", "ab", "abc"], [1, 3] },

            // Identical letter sets are connected to each other.
            { ["ab", "ab", "xyz"], [2, 2] },

            // A pure REPLACE chain: {a,b} -> {a,c} -> {a,d}, no popcount ever
            // changing, so the add/delete rule alone would leave three groups.
            { ["ab", "ac", "ad"], [1, 3] },

            // Nothing connects: six differing letters between the two sets.
            { ["abc", "xyz"], [2, 1] },

            // One word is its own group of one.
            { ["a"], [1, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GroupSizesByPairwisePopCount_LeetCodeExamples_ReturnsGroupCountAndLargestGroupSize(
        string[] words, int[] expected) =>
        Assert.Equal(expected, GroupsOfStringsSolution.GroupSizesByPairwisePopCount(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GroupSizesByHashMapNeighbors_LeetCodeExamples_ReturnsGroupCountAndLargestGroupSize(
        string[] words, int[] expected) =>
        Assert.Equal(expected, GroupsOfStringsSolution.GroupSizesByHashMapNeighbors(words));

    // LeetCode's first example, bit 0 for 'a' upward: "a" sets bit 0, "b" bit 1, "ab"
    // bits 0 and 1, and "cde" bits 2, 3 and 4.
    [Fact]
    public void LetterSetMasks_LeetCodeFirstExample_SetsOneBitPerLetterOfEachWord()
    {
        var masks = GroupsOfStringsSolution.LetterSetMasks(["a", "b", "ab", "cde"]);

        Assert.Equal([0b1, 0b10, 0b11, 0b11100], Enumerable.Range(0, masks.Length).Select(masks.Get));
    }
}
