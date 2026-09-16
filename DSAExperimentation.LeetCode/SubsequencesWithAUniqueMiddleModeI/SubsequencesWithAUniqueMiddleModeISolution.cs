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

        for (var first = 0; first < n; first++)
        {
            for (var second = first + 1; second < n; second++)
            {
                for (var middle = second + 1; middle < n; middle++)
                {
                    count += CountQualifyingRightPairs(nums, (first, second, middle));
                }
            }
        }

        return count % ModularArithmetic.Modulo;
    }

    // The five chosen indices are one length-5 subsequence, so the first three fixes
    // its middle (index 2 of the 5); this counts which choices of the remaining two,
    // both strictly after it, complete it into one whose middle value is the sole mode.
    private static long CountQualifyingRightPairs(int[] nums, (int First, int Second, int Middle) prefix)
    {
        var count = 0L;

        for (var fourth = prefix.Middle + 1; fourth < nums.Length; fourth++)
        {
            for (var fifth = fourth + 1; fifth < nums.Length; fifth++)
            {
                if (IsUniqueMiddleMode(nums, [prefix.First, prefix.Second, prefix.Middle, fourth, fifth]))
                {
                    count++;
                }
            }
        }

        return count;
    }

    // The five chosen indices are one length-5 subsequence, so they arrive as the
    // one ordered sequence they are; index 2 of it is the subsequence's middle.
    private static bool IsUniqueMiddleMode(int[] nums, int[] indices)
    {
        var frequency = CountValuesAtIndices(nums, indices);

        return IsSoleMode(frequency, nums[indices[2]]);
    }

    // How often each value appears among the five chosen indices.
    private static Dictionary<int, int> CountValuesAtIndices(int[] nums, int[] indices)
    {
        var frequency = new Dictionary<int, int>();

        foreach (var index in indices)
        {
            frequency[nums[index]] = frequency.GetValueOrDefault(nums[index]) + 1;
        }

        return frequency;
    }

    // `middleValue` is the sole mode when no other value present ties it: any other
    // value reaching its count would leave the five with two modes instead of one.
    private static bool IsSoleMode(Dictionary<int, int> frequency, int middleValue)
    {
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
        if (nums.Length < 5)
        {
            return 0;
        }

        return SweepMiddleIndices(nums);
    }

    // The sweep itself: the middle index walks left to right with the right side
    // counting the whole array and the left side still empty. Each step moves nums[i]
    // across, and - while i still has two elements on each side, so a length-5
    // subsequence can form around it - adds what that middle index contributes.
    private static long SweepMiddleIndices(int[] nums)
    {
        var leftFrequencies = new Dictionary<int, int>();
        var rightFrequencies = CountValues(nums);
        var sumSquaresLeft = 0L;
        var sumSquaresRight = SquaredCountSum(rightFrequencies);
        var answer = 0L;

        for (var i = 0; i < nums.Length; i++)
        {
            var x = nums[i];
            sumSquaresRight = WithoutValue(rightFrequencies, sumSquaresRight, x);

            if (i >= 2 && i <= nums.Length - 3)
            {
                answer = (answer + ContributionAtMiddle(
                    nums, i, (leftFrequencies, sumSquaresLeft), (rightFrequencies, sumSquaresRight)))
                    % ModularArithmetic.Modulo;
            }

            sumSquaresLeft = WithValue(leftFrequencies, sumSquaresLeft, x);
        }

        return answer;
    }

    // How often each distinct value of `values` occurs.
    private static Dictionary<int, int> CountValues(int[] values)
    {
        var frequencies = new Dictionary<int, int>();

        foreach (var value in values)
        {
            frequencies[value] = frequencies.GetValueOrDefault(value) + 1;
        }

        return frequencies;
    }

    // The sum of the squares of a side's counts - the other half of what a side
    // carries, and what AddOne and RemoveOne keep in step with the counts themselves.
    private static long SquaredCountSum(Dictionary<int, int> frequencies)
        => frequencies.Values.Sum(count => (long)count * count);

    // The right side as the sweep passes nums[i]: one fewer occurrence of that value
    // and the sum of squares kept in step, the entry dropped once none are left. The
    // counts dictionary is the caller's own and is updated in place.
    private static long WithoutValue(Dictionary<int, int> frequencies, long sumSquares, int value)
    {
        var (newCount, newSumSquares) = RemoveOne(frequencies[value], sumSquares);
        frequencies[value] = newCount;

        if (newCount == 0)
        {
            frequencies.Remove(value);
        }

        return newSumSquares;
    }

    private static (int NewCount, long NewSumSquares) RemoveOne(int count, long sumSquares)
    {
        sumSquares -= (long)count * count;
        var newCount = count - 1;
        sumSquares += (long)newCount * newCount;
        return (newCount, sumSquares);
    }

    // The middle index's own contribution: what the two sides say about it, together
    // with the shared non-x moments between them, summed over the nine (lx, rx) splits.
    private static long ContributionAtMiddle(
        int[] nums,
        int middleIndex,
        (Dictionary<int, int> Frequencies, long SumSquares) left,
        (Dictionary<int, int> Frequencies, long SumSquares) right)
    {
        var sides = MiddleSides(nums, middleIndex, left, right);
        var shared = SharedNonXMoments(nums[middleIndex], left.Frequencies, right.Frequencies);

        return SumSplitContributions(sides, shared);
    }

    // What the two sides say about middle index i: how many copies of its value each
    // holds, how many other elements each has, and how many distinct pairs each can
    // supply - the six numbers every split below is counted from. A side arrives as the
    // one pair it is: its per-value frequency counts together with their running sum of
    // squares, which AddOne and RemoveOne update as a pair.
    private static (int FreqLeft, int FreqRight, int RestLeft, int RestRight, long PairsLeft, long PairsRight)
        MiddleSides(
            int[] nums,
            int middleIndex,
            (Dictionary<int, int> Frequencies, long SumSquares) left,
            (Dictionary<int, int> Frequencies, long SumSquares) right)
    {
        var x = nums[middleIndex];
        var freqLeft = left.Frequencies.GetValueOrDefault(x);
        var freqRight = right.Frequencies.GetValueOrDefault(x);
        var restLeft = middleIndex - freqLeft;
        var restRight = (nums.Length - 1 - middleIndex) - freqRight;
        var sumSquaresLeftNonX = left.SumSquares - (long)freqLeft * freqLeft;
        var sumSquaresRightNonX = right.SumSquares - (long)freqRight * freqRight;
        var pairsLeft = (restLeft * (long)restLeft - sumSquaresLeftNonX) / 2;
        var pairsRight = (restRight * (long)restRight - sumSquaresRightNonX) / 2;

        return (freqLeft, freqRight, restLeft, restRight, pairsLeft, pairsRight);
    }

    // Every non-x value present on both sides contributes to how many ways the
    // single free pick on one side could collide with the distinct pair on the
    // other - summed once here (over whichever side has fewer distinct values)
    // rather than per (lx, rx) split.
    private static (long SharedSum, long SharedSumWeightedByRight, long SharedSumWeightedByLeft)
        SharedNonXMoments(int middleValue, Dictionary<int, int> freqLeft, Dictionary<int, int> freqRight)
    {
        var byAscendingCount = ByAscendingCount(freqLeft, freqRight);
        var smallerMapSide = ReferenceEquals(byAscendingCount.Smaller, freqLeft)
            ? SmallerMapSide.Left
            : SmallerMapSide.Right;
        var shared = (Sum: 0L, WeightedByRight: 0L, WeightedByLeft: 0L);

        foreach (var (value, smallerCount) in byAscendingCount.Smaller)
        {
            if (value == middleValue || !byAscendingCount.Larger.TryGetValue(value, out var largerCount))
            {
                continue;
            }

            var (freqLeftValue, freqRightValue) = CountsBySide(smallerMapSide, smallerCount, largerCount);

            shared = AddSharedMoments(shared, freqLeftValue, freqRightValue);
        }

        return (shared.Sum, shared.WeightedByRight, shared.WeightedByLeft);
    }

    // The two frequency maps ordered by how many distinct values each holds, so the
    // pairwise walk above always iterates the smaller of the two.
    private static FrequencyMapsByAscendingCount ByAscendingCount(
        Dictionary<int, int> freqLeft, Dictionary<int, int> freqRight)
    {
        if (freqLeft.Count <= freqRight.Count)
        {
            return new FrequencyMapsByAscendingCount(freqLeft, freqRight);
        }

        return new FrequencyMapsByAscendingCount(freqRight, freqLeft);
    }

    // A pair of counts read back in (left, right) order, whichever side the smaller
    // map turned out to be on.
    private static (int Left, int Right) CountsBySide(
        SmallerMapSide smallerMapSide, int smallerCount, int largerCount)
    {
        if (smallerMapSide == SmallerMapSide.Left)
        {
            return (smallerCount, largerCount);
        }

        return (largerCount, smallerCount);
    }

    // The three moments one value present on both sides contributes: the product of its
    // two counts, and that product weighted by the other side's count in turn.
    private static (long Sum, long WeightedByRight, long WeightedByLeft) AddSharedMoments(
        (long Sum, long WeightedByRight, long WeightedByLeft) shared,
        int freqLeftValue,
        int freqRightValue)
    {
        var product = (long)freqLeftValue * freqRightValue;

        return (shared.Sum + product,
            shared.WeightedByRight + (product * freqRightValue),
            shared.WeightedByLeft + (product * freqLeftValue));
    }

    // The nine (lx, rx) splits with lx and rx in {0, 1, 2} cover every way the four side
    // picks can include x. The two k = 1 splits are the ones whose non-x picks are a
    // single free element on one side and a distinct pair on the other - aL = 1, aR = 2
    // and its mirror - so they read off the shared non-x moments rather than a plain
    // choice; every other split is a plain (possibly repeating) choice.
    private static long SumSplitContributions(
        (int FreqLeft, int FreqRight, int RestLeft, int RestRight, long PairsLeft, long PairsRight) sides,
        (long Shared, long WeightedByRight, long WeightedByLeft) shared)
    {
        var singleThenPair = (sides.RestLeft * sides.PairsRight)
            - ((sides.RestRight * shared.Shared) - shared.WeightedByRight);
        var pairThenSingle = (sides.RestRight * sides.PairsLeft)
            - ((sides.RestLeft * shared.Shared) - shared.WeightedByLeft);
        var total = 0L;

        for (var leftXPicks = 0; leftXPicks <= 2; leftXPicks++)
        {
            for (var rightXPicks = 0; rightXPicks <= 2; rightXPicks++)
            {
                total += SplitContribution(
                    (leftXPicks, rightXPicks),
                    (sides.FreqLeft, sides.FreqRight),
                    (sides.RestLeft, sides.RestRight),
                    (singleThenPair, pairThenSingle));
            }
        }

        return ((total % ModularArithmetic.Modulo) + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
    }

    // One (lx, rx) split's contribution: how many ways the two sides' own copies of x
    // supply those counts, times how many ways the remaining non-x picks avoid tying
    // x's count. A split nothing can supply contributes nothing, and so does k = 0 -
    // with only 4 other elements, none of them can have frequency 0, so x is always
    // tied by whichever value one of them takes.
    private static long SplitContribution(
        (int LeftPicks, int RightPicks) picks,
        (int Left, int Right) freqX,
        (int Left, int Right) rest,
        (long SingleThenPair, long PairThenSingle) pairTerms)
    {
        var waysToPickXs = ChooseAtMostTwo(freqX.Left, picks.LeftPicks) * ChooseAtMostTwo(freqX.Right, picks.RightPicks);

        if (waysToPickXs == 0)
        {
            return 0;
        }

        var matchCount = picks.LeftPicks + picks.RightPicks;
        var waysToPickOthers = matchCount switch
        {
            0 => 0L,
            1 when picks.LeftPicks == 1 => pairTerms.SingleThenPair,
            1 => pairTerms.PairThenSingle,
            _ => ChooseAtMostTwo(rest.Left, 2 - picks.LeftPicks) * ChooseAtMostTwo(rest.Right, 2 - picks.RightPicks),
        };

        return waysToPickXs * waysToPickOthers;
    }

    private static long ChooseAtMostTwo(long available, int take) => take switch
    {
        0 => 1,
        1 => available >= 1 ? available : 0,
        2 => available >= 2 ? ChooseTwo(available) : 0,
        _ => throw new ArgumentOutOfRangeException(nameof(take)),
    };

    // The number of unordered pairs a pool of this size can supply.
    private static long ChooseTwo(long available) => available * (available - 1) / 2;

    // The left side as the sweep moves past nums[i] - the mirror of WithoutValue, with
    // the same in-place counts dictionary and the same returned sum of squares.
    private static long WithValue(Dictionary<int, int> frequencies, long sumSquares, int value)
    {
        var (newCount, newSumSquares) = AddOne(frequencies.GetValueOrDefault(value), sumSquares);
        frequencies[value] = newCount;

        return newSumSquares;
    }

    private static (int NewCount, long NewSumSquares) AddOne(int count, long sumSquares)
    {
        sumSquares -= (long)count * count;
        var newCount = count + 1;
        sumSquares += (long)newCount * newCount;
        return (newCount, sumSquares);
    }
}
