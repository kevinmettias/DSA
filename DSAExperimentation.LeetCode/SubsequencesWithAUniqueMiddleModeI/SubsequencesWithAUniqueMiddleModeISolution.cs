using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.SubsequencesWithAUniqueMiddleModeI;

// LeetCode 3395. Subsequences with a Unique Middle Mode I: count length-5
// subsequences of nums whose middle element (index 2 of the 5) is the sole mode
// of the 5, reported modulo 1e9+7.
//
// For a fixed middle index i with value x, the subsequence always takes exactly
// 2 elements from before i and 2 from after i (order is fixed by subsequence
// position). Splitting those 4 by how many equal x (lx from the left, rx from
// the right, k = lx + rx):
//   - k in {3, 4}: x's total count is 4 or 5 - already a strict majority of 5,
//     so the other picks are unconstrained.
//   - k == 2: x's count is 3, still a strict majority over any single other
//     value (which can repeat at most twice) - unconstrained.
//   - k == 1: x's count is 2, so the other 3 picks must ALL be pairwise
//     distinct in value (any repeat among them would tie x at 2).
//   - k == 0: x's count is only 1, and the other 4 picks are always present -
//     one of them always ties x's count of 1, so x can never be the sole mode.
//     This case contributes nothing, however the other 4 are distributed.
// This repo's own Domain.Modular.ModularArithmetic supplies the modulus every
// counting problem here reports against.
internal static class SubsequencesWithAUniqueMiddleModeISolution
{
    // Textbook baseline: every combination of 5 indices, mode computed directly
    // from a per-combination frequency count. O(n^5) - the arm the O(n^2)
    // combinatorial strategy has to beat.
    public static long CountMiddleModeSubsequencesByBruteForce(int[] nums)
    {
        var n = nums.Length;
        var count = 0L;

        for (var i0 = 0; i0 < n; i0++)
        {
            for (var i1 = i0 + 1; i1 < n; i1++)
            {
                for (var i2 = i1 + 1; i2 < n; i2++)
                {
                    for (var i3 = i2 + 1; i3 < n; i3++)
                    {
                        for (var i4 = i3 + 1; i4 < n; i4++)
                        {
                            if (IsUniqueMiddleMode(nums, i0, i1, i2, i3, i4))
                            {
                                count++;
                            }
                        }
                    }
                }
            }
        }

        return count % ModularArithmetic.Modulo;
    }

    private static bool IsUniqueMiddleMode(int[] nums, int i0, int i1, int i2, int i3, int i4)
    {
        var frequency = new Dictionary<int, int>();

        foreach (var index in new[] { i0, i1, i2, i3, i4 })
        {
            frequency[nums[index]] = frequency.GetValueOrDefault(nums[index]) + 1;
        }

        var middleValue = nums[i2];
        var middleFrequency = frequency[middleValue];

        return frequency.All(entry => entry.Key == middleValue || entry.Value < middleFrequency);
    }

    // Composed: sweep the middle index left to right, maintaining running
    // per-value frequency counts (and their sums of squares) for the elements
    // strictly before and strictly after it. For each middle index, 9 (lx, rx)
    // splits with lx, rx in {0, 1, 2} cover every way the 4 side picks can
    // include x; each split's non-x count is either a plain (possibly
    // repeating) choice or a distinct-valued choice depending on k = lx + rx,
    // both closed forms over the running frequency sums - no per-i enumeration
    // of actual elements. O(n) side-value bookkeeping per index against the
    // shared values between the two sides, O(n^2) total against the baseline's
    // O(n^5).
    public static long CountMiddleModeSubsequencesByModularCombinatorics(int[] nums)
    {
        var n = nums.Length;

        if (n < 5)
        {
            return 0;
        }

        var freqRight = new Dictionary<int, int>();

        foreach (var value in nums)
        {
            freqRight[value] = freqRight.GetValueOrDefault(value) + 1;
        }

        var sumSquaresRight = freqRight.Values.Sum(count => (long)count * count);
        var freqLeft = new Dictionary<int, int>();
        var sumSquaresLeft = 0L;
        var answer = 0L;

        for (var i = 0; i < n; i++)
        {
            var x = nums[i];

            (freqRight[x], sumSquaresRight) = RemoveOne(freqRight[x], sumSquaresRight);

            if (freqRight[x] == 0)
            {
                freqRight.Remove(x);
            }

            if (i >= 2 && i <= n - 3)
            {
                var contribution = ContributionAtMiddle(x, i, n, freqLeft, sumSquaresLeft, freqRight, sumSquaresRight);
                answer = (answer + contribution) % ModularArithmetic.Modulo;
            }

            var oldLeftCount = freqLeft.GetValueOrDefault(x);
            (freqLeft[x], sumSquaresLeft) = AddOne(oldLeftCount, sumSquaresLeft);
        }

        return answer;
    }

    private static (int NewCount, long NewSumSquares) RemoveOne(int count, long sumSquares)
    {
        sumSquares -= (long)count * count;
        var newCount = count - 1;
        sumSquares += (long)newCount * newCount;
        return (newCount, sumSquares);
    }

    private static (int NewCount, long NewSumSquares) AddOne(int count, long sumSquares)
    {
        sumSquares -= (long)count * count;
        var newCount = count + 1;
        sumSquares += (long)newCount * newCount;
        return (newCount, sumSquares);
    }

    private static long ContributionAtMiddle(
        int x, int i, int n, Dictionary<int, int> freqLeft, long sumSquaresLeft, Dictionary<int, int> freqRight, long sumSquaresRight)
    {
        var freqLeftX = freqLeft.GetValueOrDefault(x);
        var freqRightX = freqRight.GetValueOrDefault(x);
        var restLeft = i - freqLeftX;
        var restRight = (n - 1 - i) - freqRightX;
        var sumSquaresLeftNonX = sumSquaresLeft - (long)freqLeftX * freqLeftX;
        var sumSquaresRightNonX = sumSquaresRight - (long)freqRightX * freqRightX;
        var distinctPairsLeft = (restLeft * (long)restLeft - sumSquaresLeftNonX) / 2;
        var distinctPairsRight = (restRight * (long)restRight - sumSquaresRightNonX) / 2;

        var (sharedSum, sharedSumWeightedByRight, sharedSumWeightedByLeft) = SharedNonXMoments(x, freqLeft, freqRight);

        // aL = 1, aR = 2: one free pick on the left, a distinct pair on the
        // right, none of the 3 sharing a value (k = 1 via the left's single x).
        var leftSingleRightPair =
            restLeft * distinctPairsRight - (restRight * sharedSum - sharedSumWeightedByRight);

        // aL = 2, aR = 1: the mirror image (k = 1 via the right's single x).
        var leftPairRightSingle =
            restRight * distinctPairsLeft - (restLeft * sharedSum - sharedSumWeightedByLeft);

        var total = 0L;

        for (var leftXPicks = 0; leftXPicks <= 2; leftXPicks++)
        {
            for (var rightXPicks = 0; rightXPicks <= 2; rightXPicks++)
            {
                var waysToPickXs = ChooseAtMostTwo(freqLeftX, leftXPicks) * ChooseAtMostTwo(freqRightX, rightXPicks);

                if (waysToPickXs == 0)
                {
                    continue;
                }

                var matchCount = leftXPicks + rightXPicks;

                // k = 0 (x's own count stays 1) can never be a strict sole mode:
                // with only 4 other elements, none of them can have frequency 0,
                // so x is always tied by whichever value one of them takes.
                var waysToPickOthers = matchCount switch
                {
                    0 => 0,
                    1 when leftXPicks == 1 => leftSingleRightPair,
                    1 => leftPairRightSingle,
                    _ => ChooseAtMostTwo(restLeft, 2 - leftXPicks) * ChooseAtMostTwo(restRight, 2 - rightXPicks),
                };

                total += waysToPickXs * waysToPickOthers;
            }
        }

        return ((total % ModularArithmetic.Modulo) + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
    }

    // Every non-x value present on both sides contributes to how many ways the
    // single free pick on one side could collide with the distinct pair on the
    // other - summed once here (over whichever side has fewer distinct values)
    // rather than per (lx, rx) split.
    private static (long SharedSum, long SharedSumWeightedByRight, long SharedSumWeightedByLeft)
        SharedNonXMoments(int x, Dictionary<int, int> freqLeft, Dictionary<int, int> freqRight)
    {
        var (smaller, larger) = freqLeft.Count <= freqRight.Count ? (freqLeft, freqRight) : (freqRight, freqLeft);
        var smallerIsLeft = ReferenceEquals(smaller, freqLeft);

        var sharedSum = 0L;
        var sharedSumWeightedByRight = 0L;
        var sharedSumWeightedByLeft = 0L;

        foreach (var (value, smallerCount) in smaller)
        {
            if (value == x || !larger.TryGetValue(value, out var largerCount))
            {
                continue;
            }

            var (freqLeftValue, freqRightValue) = smallerIsLeft ? (smallerCount, largerCount) : (largerCount, smallerCount);
            var product = (long)freqLeftValue * freqRightValue;

            sharedSum += product;
            sharedSumWeightedByRight += product * freqRightValue;
            sharedSumWeightedByLeft += product * freqLeftValue;
        }

        return (sharedSum, sharedSumWeightedByRight, sharedSumWeightedByLeft);
    }

    private static long ChooseAtMostTwo(long available, int take) => take switch
    {
        0 => 1,
        1 => available >= 1 ? available : 0,
        2 => available >= 2 ? available * (available - 1) / 2 : 0,
        _ => throw new ArgumentOutOfRangeException(nameof(take)),
    };
}
