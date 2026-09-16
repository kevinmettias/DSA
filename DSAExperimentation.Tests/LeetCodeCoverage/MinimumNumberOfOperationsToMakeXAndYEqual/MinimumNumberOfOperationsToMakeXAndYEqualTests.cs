using DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeXAndYEqual;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfOperationsToMakeXAndYEqual;

// Harness only: the algorithms live in
// MinimumNumberOfOperationsToMakeXAndYEqualSolution. One test method per strategy
// over one shared set of LeetCode's own examples, so a failure names the strategy
// that broke (TwoSumTests precedent).
public sealed class MinimumNumberOfOperationsToMakeXAndYEqualTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 26, 1, 3 },
            { 54, 2, 4 },
            { 25, 30, 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByMutationQueue_LeetCodeExamples_ReturnsFewestOperations(int x, int y, int expected)
    {
        var actual = MinimumNumberOfOperationsToMakeXAndYEqualSolution.MinOperationsByMutationQueue(x, y);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByMemoizedReduce_LeetCodeExamples_ReturnsFewestOperations(int x, int y, int expected)
    {
        var actual = MinimumNumberOfOperationsToMakeXAndYEqualSolution.MinOperationsByMemoizedReduce(x, y);
        Assert.Equal(expected, actual);
    }
}
