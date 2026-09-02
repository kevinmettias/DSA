using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ThresholdMajorityQueries;

// A static range-mode index (sqrt decomposition): every query this problem asks
// reduces to "what's the most frequent element of nums[l..r], and does it clear
// threshold" (see ThresholdMajorityQueriesSolution's own comment for why the
// threshold never changes which element wins). This answers that reduced query,
// which is why it lives beside the solution rather than in Domain/ - the block-
// mode table and the tie-break rule are this one problem's shape, not a reusable
// data structure.
//
// Every value's occurrence positions are kept sorted (built by one left-to-right
// scan), so an exact frequency in any [l, r] is one BinarySearch.LowerBound and one
// UpperBound apart - the same IRandomAccessSequence composition IntervalSet uses
// (ARCHITECTURE.md's sanctioned Searching-as-a-client edge).
internal sealed class ThresholdMajorityBlockIndex
{
    private readonly int[] _nums;
    private readonly int _blockSize;
    private readonly int _blockCount;
    private readonly HashMap<int, DynamicArray<int>> _positions;

    // [startBlock, endBlock] (inclusive, startBlock <= endBlock) -> the mode of the
    // aligned index range those blocks cover, ties broken to the smallest value -
    // built once by extending a running frequency count one block at a time.
    private readonly (int Value, int Freq)[,] _blockMode;

    private ThresholdMajorityBlockIndex(
        int[] nums, int blockSize, int blockCount,
        HashMap<int, DynamicArray<int>> positions, (int Value, int Freq)[,] blockMode)
    {
        _nums = nums;
        _blockSize = blockSize;
        _blockCount = blockCount;
        _positions = positions;
        _blockMode = blockMode;
    }

    public static ThresholdMajorityBlockIndex Build(int[] nums)
    {
        var blockSize = Math.Max(1, (int)Math.Sqrt(nums.Length));
        var blockCount = (nums.Length + blockSize - 1) / blockSize;

        return new ThresholdMajorityBlockIndex(
            nums, blockSize, blockCount, BuildPositions(nums), BuildBlockMode(nums, blockSize, blockCount));
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
                var from = endBlock * blockSize;
                var to = Math.Min(nums.Length, from + blockSize);

                for (var i = from; i < to; i++)
                {
                    freq.TryGetValue(nums[i], out var count);
                    count++;
                    freq.Set(nums[i], count);
                    mode = PreferHigherFreqThenSmallerValue(mode, (nums[i], count));
                }

                blockMode[startBlock, endBlock] = mode;
            }
        }

        return blockMode;
    }

    // The element with frequency >= threshold and the highest frequency (smallest
    // value on tie) in nums[l..r], or LeetCodeAnswer.None if no element reaches
    // threshold. Candidates: the mode of whichever full blocks lie entirely inside
    // [l, r] (its full-range frequency re-verified exactly, since it may also occur
    // in the boundary), plus every element in the up-to-two partial blocks at the
    // ends. ThresholdMajorityQueriesSolution's own comment proves these candidates
    // always include the true answer.
    public int Query(int l, int r, int threshold)
    {
        var leftBlock = l / _blockSize;
        var rightBlock = r / _blockSize;
        var best = (Value: 0, Freq: 0);

        if (leftBlock == rightBlock)
        {
            for (var i = l; i <= r; i++)
            {
                best = ConsiderCandidate(best, _nums[i], l, r);
            }

            return best.Freq >= threshold ? best.Value : LeetCodeAnswer.None;
        }

        var leftBoundaryEnd = Math.Min(r, (((leftBlock + 1) * _blockSize) - 1));

        for (var i = l; i <= leftBoundaryEnd; i++)
        {
            best = ConsiderCandidate(best, _nums[i], l, r);
        }

        var rightBoundaryStart = Math.Max(l, rightBlock * _blockSize);

        for (var i = rightBoundaryStart; i <= r; i++)
        {
            best = ConsiderCandidate(best, _nums[i], l, r);
        }

        if (leftBlock + 1 <= rightBlock - 1)
        {
            var (alignedCandidate, _) = _blockMode[leftBlock + 1, rightBlock - 1];
            best = ConsiderCandidate(best, alignedCandidate, l, r);
        }

        return best.Freq >= threshold ? best.Value : LeetCodeAnswer.None;
    }

    private (int Value, int Freq) ConsiderCandidate((int Value, int Freq) best, int value, int l, int r) =>
        PreferHigherFreqThenSmallerValue(best, (value, FrequencyInRange(value, l, r)));

    private static (int Value, int Freq) PreferHigherFreqThenSmallerValue(
        (int Value, int Freq) current, (int Value, int Freq) candidate) =>
        candidate.Freq > current.Freq || (candidate.Freq == current.Freq && candidate.Value < current.Value)
            ? candidate
            : current;

    private int FrequencyInRange(int value, int l, int r)
    {
        if (!_positions.TryGetValue(value, out var positionList))
        {
            return 0;
        }

        var sequence = new DynamicArraySequence<int>(positionList);
        var lower = BinarySearch.LowerBound<int, DynamicArraySequence<int>>(sequence, l);
        var upper = BinarySearch.UpperBound<int, DynamicArraySequence<int>>(sequence, r);

        return upper - lower;
    }
}
