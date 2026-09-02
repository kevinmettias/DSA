using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MoveZeroes;

// LeetCode 283. Move Zeroes: move every 0 to the end of the array in place,
// preserving the relative order of the non-zero elements.
//
// The naive baseline rescans from i for the next non-zero every time a zero is
// found - O(n^2) worst case when zeroes cluster at the front. The composed
// strategy below is the same ArrayIndexedSequence<T> Get/Set two-pointer swap
// SortColors' Dutch-flag partition already proves out for this repo,
// specialized to a single zero/nonzero split instead of a three-way one.
internal static class MoveZeroesSolution
{
    // The textbook answer: for each zero found, scan forward for the next
    // non-zero and swap it into place. Deliberately written without this
    // repo's primitives - it is the arm the composed strategy below has to
    // justify itself against.
    public static void MoveZeroesToEndByLinearScan(int[] nums)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            if (nums[i] != 0)
            {
                continue;
            }

            var j = i + 1;

            while (j < nums.Length && nums[j] == 0)
            {
                j++;
            }

            if (j < nums.Length)
            {
                (nums[i], nums[j]) = (nums[j], nums[i]);
            }
        }
    }

    // Two-pointer swap over this repo's own ArrayIndexedSequence<int>:
    // insertPos tracks where the next non-zero belongs, so every non-zero is
    // swapped into place in a single left-to-right pass.
    public static void MoveZeroesToEndByArrayIndexedTwoPointer(int[] nums)
    {
        var seq = new ArrayIndexedSequence<int>(nums);
        var insertPos = 0;

        for (var i = 0; i < seq.Length; i++)
        {
            if (seq.Get(i) != 0)
            {
                Swap(seq, insertPos++, i);
            }
        }
    }

    private static void Swap(ArrayIndexedSequence<int> seq, int first, int second)
    {
        var temp = seq.Get(first);
        seq.Set(first, seq.Get(second));
        seq.Set(second, temp);
    }
}
