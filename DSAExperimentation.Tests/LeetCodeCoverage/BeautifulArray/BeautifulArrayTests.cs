using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BeautifulArray;

// LeetCode 932. Beautiful Array: top-down divide-and-conquer construction - if
// Build(m) is a beautiful permutation of 1..m, then doubling-and-shifting its
// values into odds (2x-1) and evens (2x) keeps the "no A[k] is the average of an
// earlier/later pair" property, since an odd value can never average with an even
// one to produce an odd result on one side and the parity split itself already
// keeps every odd strictly before/after being irrelevant to every even comparison.
// This repo's own HashMap<int,int[]> memoizes the O(n) distinct subproblem sizes
// the recursion actually revisits (Build(n) and Build(n/2) both recurse into
// overlapping halves) - the same top-down-memoization role HashMap already plays
// elsewhere in this repo's DP coverage, not just Two Sum's single O(n) lookup.
public sealed partial class BeautifulArrayTests
{
    [Fact]
    public void Construct_LengthOne_ReturnsSingleElement()
    {
        Assert.Equal([1], Construct(1));
    }

    [Fact]
    public void Construct_LengthFour_MatchesKnownBeautifulPermutation()
    {
        Assert.Equal([1, 3, 2, 4], Construct(4));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(17)]
    public void Construct_VariousLengths_ProducesABeautifulPermutation(int n)
    {
        var result = Construct(n);
        var expectedValues = Enumerable.Range(1, n);

        Assert.Equal(expectedValues, result.OrderBy(x => x));
        Assert.True(IsBeautiful(result));
    }

    private static int[] Construct(int n) => Build(n, new HashMap<int, int[]>());

    private static int[] Build(int n, HashMap<int, int[]> memo)
    {
        if (n == 1)
        {
            return [1];
        }

        if (memo.TryGetValue(n, out var cached))
        {
            return cached;
        }

        var odds = Build((n + 1) / 2, memo).Select(x => (2 * x) - 1);
        var evens = Build(n / 2, memo).Select(x => 2 * x);
        var result = odds.Concat(evens).ToArray();

        memo.Set(n, result);
        return result;
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
