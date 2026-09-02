using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeTheMaximumOfTwoArrays;

// LeetCode 2513. Minimize the Maximum of Two Arrays: "binary search on the answer"
// over the shared maximum m - feasibility of building arr1/arr2 under some cap m is
// monotone (a larger cap only ever adds more eligible numbers), so the smallest
// feasible m is the leftmost "true" in an implicit [false...false, true...true]
// sequence over m in [1, 2*(uniqueCnt1+uniqueCnt2)]. Same FeasibleMaximumSequence +
// BinarySearch.LowerBound shape KokoEatingBananasTests/SplitArrayLargestSumTests
// already establish for their own search-on-answer.
//
// Feasibility itself is the standard inclusion-exclusion count: of the numbers in
// [1,m], eligible1 = those not divisible by divisor1 (candidates for arr1),
// eligible2 = those not divisible by divisor2 (candidates for arr2), and
// eligibleEither = those not divisible by lcm(divisor1,divisor2) (divisible by
// neither divisor, so usable by whichever array still needs them). m works iff
// eligible1 >= uniqueCnt1, eligible2 >= uniqueCnt2, and eligibleEither covers both
// counts at once.
public sealed partial class MinimizeTheMaximumOfTwoArraysTests
{
    [Theory]
    [InlineData(2, 7, 1, 3, 4)]
    [InlineData(3, 5, 2, 1, 3)]
    [InlineData(2, 4, 8, 2, 15)]
    public void MinimizeSet_LeetCodeExamples_ReturnsSmallestFeasibleMaximum(
        int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2, int expected)
    {
        var actual = MinimizeSet(divisor1, divisor2, uniqueCnt1, uniqueCnt2);
        Assert.Equal(expected, actual);
    }

    private static int MinimizeSet(int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2)
    {
        var sequence = new FeasibleMaximumSequence(divisor1, divisor2, uniqueCnt1, uniqueCnt2);
        return 1 + BinarySearch.LowerBound(sequence, true);
    }

    private static bool IsFeasible(int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2, int max)
    {
        var lcm = Lcm(divisor1, divisor2);
        var eligible1 = max - (max / divisor1);
        var eligible2 = max - (max / divisor2);
        var eligibleEither = max - (max / lcm);

        return eligible1 >= uniqueCnt1 && eligible2 >= uniqueCnt2 && eligibleEither >= uniqueCnt1 + (long)uniqueCnt2;
    }

    private static long Lcm(int a, int b) => (long)a / Gcd(a, b) * b;

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    private readonly struct FeasibleMaximumSequence(int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2)
        : IRandomAccessSequence<bool>
    {
        public int Length => (2 * (uniqueCnt1 + uniqueCnt2)) + 1;

        public bool Get(int index) => IsFeasible(divisor1, divisor2, uniqueCnt1, uniqueCnt2, 1 + index);
    }
}
