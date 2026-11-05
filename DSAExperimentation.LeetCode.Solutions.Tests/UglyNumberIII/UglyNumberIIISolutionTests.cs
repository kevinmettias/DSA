using DSAExperimentation.LeetCode.Solutions.UglyNumberIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.UglyNumberIII;

// Harness only: both strategies live in UglyNumberIIISolution and are asserted
// against the same cases - the case where all three factors coincide, and the
// first-element case where the answer is just the smallest factor. LeetCode's
// first three published examples run in CatalogCaseTests; the fourth, which the
// catalog lacks, is asserted below.
public sealed partial class UglyNumberIIISolutionTests
{
    public static TheoryData<UglyCase> Cases =>
        new()
        {
            // The first requested number is always the smallest factor - and here
            // lcm(4, 6) already exceeds the search window, the case the capped
            // three-way LCM exists for.
            { new UglyCase(N: 1, A: 4, B: 6, C: 9, Expected: 4) },

            // All three factors equal: every LCM collapses onto the same set.
            { new UglyCase(N: 6, A: 3, B: 3, C: 3, Expected: 18) },
        };

    // LeetCode's fourth published example, which the catalog does not carry. Its
    // answer is close to 2e9, so only the binary search is asserted against it - the
    // counting walk is O(answer) and would spend two billion steps arriving at the
    // same number.
    public static TheoryData<UglyCase> LargeExamples =>
        new()
        {
            { new UglyCase(N: 1_000_000_000, A: 2, B: 217_983_653, C: 336_916_467, Expected: 1_999_999_984) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NthUglyNumberByCountScan_Cases_ReturnsNthMultipleOfAnyFactor(UglyCase testCase)
    {
        var actual = UglyNumberIIISolution.NthUglyNumberByCountScan(testCase.N, testCase.A, testCase.B, testCase.C);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void NthUglyNumberByBinarySearch_Cases_ReturnsNthMultipleOfAnyFactor(UglyCase testCase)
    {
        var actual = UglyNumberIIISolution.NthUglyNumberByBinarySearch(testCase.N, testCase.A, testCase.B, testCase.C);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(LargeExamples))]
    public void NthUglyNumberByBinarySearch_BillionthUglyNumber_CountsWithoutOverflowingTheLcm(UglyCase testCase)
    {
        var actual = UglyNumberIIISolution.NthUglyNumberByBinarySearch(testCase.N, testCase.A, testCase.B, testCase.C);

        Assert.Equal(testCase.Expected, actual);
    }

    // A window past int around an answer inside it. With factors 3, 4 and 6 the ugly
    // numbers are the multiples of 3 or 4: six in every twelve integers (3, 4, 6, 8, 9,
    // 12, then the same pattern shifted by 12), so the n-th for n = 6k + 4 is 12k + 8.
    // 10^9 = 6 * 166_666_666 + 4, which puts the answer at exactly 2 * 10^9, LeetCode's
    // own limit - while the window searched, rank times the smallest factor, is
    // 3 * 10^9, past int.
    [Fact]
    public void NthUglyNumberByBinarySearch_WindowPastIntRange_ReturnsAnswerInsideIt()
    {
        const int CompleteBlocksOfTwelve = 166_666_666;
        const int FourthUglyNumberInABlock = 8;
        var testCase = new UglyCase(
            N: 1_000_000_000, A: 3, B: 4, C: 6, Expected: (12 * CompleteBlocksOfTwelve) + FourthUglyNumberInABlock);

        var actual = UglyNumberIIISolution.NthUglyNumberByBinarySearch(testCase.N, testCase.A, testCase.B, testCase.C);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: how many multiples to count to, the three factors, and the
    // number that lands there. The five travel together at every row - three adjacent
    // ints and the count they are answered against - so they are one thing with a name
    // rather than five positional arguments a reader has to line up by hand.
    public readonly record struct UglyCase(int N, int A, int B, int C, int Expected);
}
