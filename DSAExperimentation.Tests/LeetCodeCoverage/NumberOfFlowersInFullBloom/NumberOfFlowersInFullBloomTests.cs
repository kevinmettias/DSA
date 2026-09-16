using DSAExperimentation.LeetCode.NumberOfFlowersInFullBloom;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfFlowersInFullBloom;

// Harness only. Both strategies - the O(n*m) per-person scan and the sort-then-
// bound composition - live in NumberOfFlowersInFullBloomSolution; this file pins
// each of them to LeetCode's published examples plus the interval endpoints the
// bound strategy has to split exactly right (a person arriving before every start,
// after every end, and on a start and an end).
public sealed class NumberOfFlowersInFullBloomTests
{
    public static TheoryData<int[][], int[], int[]> Examples =>
        new()
        {
            { [[1, 6], [3, 7], [9, 12], [4, 13]], [2, 3, 7, 11], [1, 2, 2, 2] },
            { [[1, 10], [3, 3]], [3, 3, 2], [2, 2, 1] },
            { [[5, 10]], [1], [0] },
            { [[1, 3]], [4], [0] },
            { [[1, 2], [2, 3], [3, 4]], [2], [2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FullBloomFlowersByPerPersonScan_LeetCodeExamples_ReturnsCountPerPerson(
        int[][] flowers, int[] persons, int[] expected)
    {
        var actual = NumberOfFlowersInFullBloomSolution.FullBloomFlowersByPerPersonScan(flowers, persons);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FullBloomFlowersBySortedBounds_LeetCodeExamples_ReturnsCountPerPerson(
        int[][] flowers, int[] persons, int[] expected)
    {
        var actual = NumberOfFlowersInFullBloomSolution.FullBloomFlowersBySortedBounds(flowers, persons);

        Assert.Equal(expected, actual);
    }
}
