using DSAExperimentation.LeetCode.BeautifulArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BeautifulArray;

// Harness only. Both constructions are BeautifulArraySolution's.
//
// LC 932 is checker-judged - ANY permutation of 1..n with no element equal to the
// average of one before it and one after it is accepted - so the assertion is the
// property itself rather than one fixed array. That is not a weakening: pinning the
// divide-and-conquer output would reject the backtracking arm's equally valid
// answer, which first differs at n = 8.
public sealed class BeautifulArrayTests
{
    // Lengths both arms are checked at. The backtracking arm is exponential (n = 16
    // already costs ~13M search nodes), so the shared set stops at 12 and the
    // scalable arm gets its own larger cases below.
    public static TheoryData<int> Examples =>
        new() { 1, 2, 3, 4, 5, 6, 8, 10, 12 };

    // Sizes only the memoized construction can reach in test time; 17 is the
    // largest case the pre-migration test carried.
    public static TheoryData<int> LargeExamples =>
        new() { 17, 33, 64 };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConstructByPrunedBacktracking_LeetCodeLengths_ReturnsABeautifulPermutation(int n) =>
        AssertBeautifulPermutation(n, BeautifulArraySolution.ConstructByPrunedBacktracking(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConstructByMemoizedDivideAndConquer_LeetCodeLengths_ReturnsABeautifulPermutation(int n) =>
        AssertBeautifulPermutation(n, BeautifulArraySolution.ConstructByMemoizedDivideAndConquer(n));

    [Theory]
    [MemberData(nameof(LargeExamples))]
    public void ConstructByMemoizedDivideAndConquer_LargeLengths_ReturnsABeautifulPermutation(int n) =>
        AssertBeautifulPermutation(n, BeautifulArraySolution.ConstructByMemoizedDivideAndConquer(n));

    private static void AssertBeautifulPermutation(int n, int[] actual)
    {
        Assert.Equal(Enumerable.Range(1, n), actual.OrderBy(value => value));
        Assert.True(IsBeautiful(actual), $"Not a beautiful array: [{string.Join(", ", actual)}]");
    }

    private static bool IsBeautiful(int[] values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            for (var k = i + 1; k < values.Length; k++)
            {
                if (HasAveragingPair(values, i, k))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool HasAveragingPair(int[] values, int i, int k)
    {
        for (var j = k + 1; j < values.Length; j++)
        {
            if ((2 * values[k]) == values[i] + values[j])
            {
                return true;
            }
        }

        return false;
    }
}
