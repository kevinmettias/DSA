using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ThresholdMajorityQueries;

// A static range-mode index (sqrt decomposition): every query this problem asks
// reduces to "what's the most frequent element of nums[leftIndex..rightIndex], and does it clear
// threshold" (see ThresholdMajorityQueriesSolution's own comment for why the
// threshold never changes which element wins). This answers that reduced query,
// which is why it lives beside the solution rather than in Domain/ - the block-
// mode table and the tie-break rule are this one problem's shape, not a reusable
// data structure.
//
// Every value's occurrence positions are kept sorted (built by one left-to-right
// scan), so an exact frequency in any [leftIndex, rightIndex] is one BinarySearch.LowerBound and one
// UpperBound apart - the same IRandomAccessSequence composition IntervalSet uses
// (ARCHITECTURE.md's sanctioned Searching-as-a-client edge).
internal sealed class ThresholdMajorityBlockIndex(
    int[] nums, int blockSize,
    HashMap<int, DynamicArray<int>> positions,
    // [startBlock, endBlock] (inclusive, startBlock <= endBlock) -> the mode of the
    // aligned index range those blocks cover, ties broken to the smallest value -
    // built once by extending a running frequency count one block at a time.
    (int Value, int Freq)[,] blockMode)
{
    public static ThresholdMajorityBlockIndex Build(int[] nums)
    {
        var blockSize = Math.Max(1, (int)Math.Sqrt(nums.Length));
        var blockCount = (nums.Length + blockSize - 1) / blockSize;

        return new ThresholdMajorityBlockIndex(
            nums, blockSize, BuildPositions(nums), BuildBlockMode(nums, blockSize, blockCount));
    }

    private static HashMap<int, DynamicArray<int>> BuildPositions(int[] nums)
    {
        var positions = new HashMap<int, DynamicArray<int>>();

        for (var i = 0; i < nums.Length; i++)
        {
            if (!positions.TryGetValue(nums[i], out var list))
            {
                list = new DynamicArray<int>();
                positions.Set(nums[i], list);
            }

            list.Add(i);
        }

        return positions;
    }

    private static (int Value, int Freq)[,] BuildBlockMode(int[] nums, int blockSize, int blockCount)
    {
        var blockMode = new (int Value, int Freq)[blockCount, blockCount];

        for (var startBlock = 0; startBlock < blockCount; startBlock++)
        {
            var freq = new HashMap<int, int>();
            var mode = (Value: 0, Freq: 0);

            for (var endBlock = startBlock; endBlock < blockCount; endBlock++)
            {
                mode = ExtendModeByBlock((nums, blockSize), freq, mode, endBlock);
                blockMode[startBlock, endBlock] = mode;
            }
        }

        return blockMode;
    }

    // Folds one more block of the array into the running frequency count, reporting
    // the mode of every value counted so far - higher frequency wins, smaller value
    // breaks the tie.
    private static (int Value, int Freq) ExtendModeByBlock(
        (int[] Nums, int BlockSize) block, HashMap<int, int> freq, (int Value, int Freq) mode, int endBlock)
    {
        var from = endBlock * block.BlockSize;
        var to = Math.Min(block.Nums.Length, from + block.BlockSize);
        for (var i = from; i < to; i++)
        {
            freq.TryGetValue(block.Nums[i], out var count);
            count++;
            freq.Set(block.Nums[i], count);
            mode = PreferHigherFreqThenSmallerValue(mode, (block.Nums[i], count));
        }

        return mode;
    }

    // The element with frequency >= threshold and the highest frequency (smallest
    // value on tie) in nums[leftIndex..rightIndex], or LeetCodeAnswer.None if no element
    // reaches threshold. Candidates: the mode of whichever full blocks lie entirely
    // inside [leftIndex, rightIndex] (its full-range frequency re-verified exactly,
    // since it may also occur in the boundary), plus every element in the up-to-two
    // partial blocks at the ends. ThresholdMajorityQueriesSolution's own comment
    // proves these candidates always include the true answer.
    public int Query(int leftIndex, int rightIndex, int threshold)
    {
        var best = BestCandidateInRange(leftIndex, rightIndex);

        return best.Freq >= threshold ? best.Value : LeetCodeAnswer.None;
    }

    // Every candidate the range offers, folded into one best: the elements of the
    // up-to-two partial blocks at the ends, plus the mode of whichever full blocks lie
    // entirely inside - its full-range frequency re-verified exactly, since it may
    // also occur in a boundary. A range inside a single block has no full blocks and
    // no boundary split, so its one scan is the whole answer.
    private (int Value, int Freq) BestCandidateInRange(int leftIndex, int rightIndex)
    {
        var leftBlock = leftIndex / blockSize;
        var rightBlock = rightIndex / blockSize;
        var best = (Value: 0, Freq: 0);

        if (leftBlock == rightBlock)
        {
            return BestInRange(best, leftIndex, rightIndex, (leftIndex, rightIndex));
        }

        var leftBoundaryEnd = Math.Min(rightIndex, (((leftBlock + 1) * blockSize) - 1));
        var rightBoundaryStart = Math.Max(leftIndex, rightBlock * blockSize);
        best = BestInRange(best, leftIndex, leftBoundaryEnd, (leftIndex, rightIndex));
        best = BestInRange(best, rightBoundaryStart, rightIndex, (leftIndex, rightIndex));

        if (leftBlock + 1 <= rightBlock - 1)
        {
            var (alignedCandidate, _) = blockMode[leftBlock + 1, rightBlock - 1];
            best = ConsiderCandidate(best, alignedCandidate, leftIndex, rightIndex);
        }

        return best;
    }

    // Folds every value in [from, to] into the running best candidate. Frequencies are
    // always measured over the query's own [l, r], not over the slice being folded.
    private (int Value, int Freq) BestInRange(
        (int Value, int Freq) best, int from, int to, (int LeftIndex, int RightIndex) query)
    {
        for (var i = from; i <= to; i++)
        {
            best = ConsiderCandidate(best, nums[i], query.LeftIndex, query.RightIndex);
        }

        return best;
    }

    private (int Value, int Freq) ConsiderCandidate(
        (int Value, int Freq) best, int value, int leftIndex, int rightIndex) =>
        PreferHigherFreqThenSmallerValue(best, (value, FrequencyInRange(value, leftIndex, rightIndex)));

    private int FrequencyInRange(int value, int leftIndex, int rightIndex)
    {
        if (!positions.TryGetValue(value, out var positionList))
        {
            return 0;
        }

        var sequence = new DynamicArraySequence<int>(positionList);
        var lower = BinarySearch.LowerBound<int, DynamicArraySequence<int>>(sequence, leftIndex);
        var upper = BinarySearch.UpperBound<int, DynamicArraySequence<int>>(sequence, rightIndex);

        return upper - lower;
    }

    // The better candidate carries the higher frequency, or the smaller value when
    // the two frequencies tie.
    private static bool IsBetterCandidate((int Value, int Freq) current, (int Value, int Freq) candidate) =>
        candidate.Freq > current.Freq || (candidate.Freq == current.Freq && candidate.Value < current.Value);

    private static (int Value, int Freq) PreferHigherFreqThenSmallerValue(
        (int Value, int Freq) current, (int Value, int Freq) candidate) =>
        IsBetterCandidate(current, candidate)
            ? candidate
            : current;
}
