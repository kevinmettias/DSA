using DSAExperimentation.LeetCode.SortedGcdPairQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortedGcdPairQueries;

// Harness only. GcdPairCountIndex and both search strategies are
// SortedGcdPairQueriesSolution's - this file just pins them to LeetCode's
// published examples.
public sealed class SortedGcdPairQueriesTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [2, 3, 4], [0, 2, 2], [1, 2, 2] },
            { [4, 4, 2, 1], [5, 3, 1, 0], [4, 2, 1, 1] },
            { [2, 2], [0, 0], [2, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerQueriesByBruteForce_LeetCodeExamples_ReturnsSortedGcdAtEachQueryIndex(
        int[] nums, int[] queries, int[] expected)
    {
        var actual = SortedGcdPairQueriesSolution.AnswerQueriesByBruteForce(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerQueriesByGcdCountingSieve_LeetCodeExamples_ReturnsSortedGcdAtEachQueryIndex(
        int[] nums, int[] queries, int[] expected)
    {
        var actual = SortedGcdPairQueriesSolution.AnswerQueriesByGcdCountingSieve(nums, queries);

        Assert.Equal(expected, actual);
    }
}
