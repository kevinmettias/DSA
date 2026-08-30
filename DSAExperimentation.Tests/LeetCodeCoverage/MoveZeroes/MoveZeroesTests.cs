using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MoveZeroes;

// LeetCode 283. Move Zeroes: the same ArrayIndexedSequence<T> Get/Set two-pointer
// swap SortColors already proves out for this repo, specialized to a single
// zero/nonzero split instead of the three-way Dutch-flag partition.
public sealed partial class MoveZeroesTests
{
    [Fact]
    public void MoveZeroesToEnd_ClassicExample_MovesZeroesToEndPreservingOrder()
    {
        int[] nums = [0, 1, 0, 3, 12];

        MoveZeroesToEnd(nums);

        Assert.Equal([1, 3, 12, 0, 0], nums);
    }

    [Fact]
    public void MoveZeroesToEnd_SingleZero_NoOp()
    {
        int[] nums = [0];

        MoveZeroesToEnd(nums);

        Assert.Equal([0], nums);
    }

    [Fact]
    public void MoveZeroesToEnd_NoZeroes_LeavesOrderUnchanged()
    {
        int[] nums = [4, 2, 7];

        MoveZeroesToEnd(nums);

        Assert.Equal([4, 2, 7], nums);
    }

    private static void MoveZeroesToEnd(int[] nums)
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
