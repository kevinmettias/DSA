using DSAExperimentation.LeetCode.Solutions.ShuffleAnArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShuffleAnArray;

// Harness only: both strategies live in ShuffleAnArraySolution. Each is held twice:
// as one shuffle, its nested class's Shuffle, which never mutates the array it was
// given (what reset() depends on) and returns a permutation of it; and as LeetCode's
// Solution object its CreateBy factory makes, whose shuffle() answers permutations
// that vary from call to call and whose reset() answers nums as given, however the
// caller treats the arrays it is handed back.
public sealed partial class ShuffleAnArraySolutionTests
{
    // Enough shuffles of five values that a strategy drawing one permutation every
    // time, out of 120, would show it.
    private const int Shuffles = 20;

    public static TheoryData<int[]> Examples =>
        new()
        {
            { [1, 2, 3] },
            { [1, 2, 3, 4, 5] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveRandomRemainingShuffle_AfterShuffling_OriginalIsUnchanged(int[] original)
    {
        var copy = (int[])original.Clone();

        ShuffleAnArraySolution.RemoveRandomRemaining.Shuffle(copy, new Random(1));

        Assert.Equal(original, copy);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveRandomRemainingShuffle_AnyOriginal_ReturnsAPermutationOfTheOriginalElements(int[] original)
    {
        var shuffled = ShuffleAnArraySolution.RemoveRandomRemaining.Shuffle(original, new Random(1));

        Assert.Equal(original.Length, shuffled.Length);
        Assert.Equal(original.OrderBy(x => x), shuffled.OrderBy(x => x));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FisherYatesDynamicArrayShuffle_AfterShuffling_OriginalIsUnchanged(int[] original)
    {
        var copy = (int[])original.Clone();

        ShuffleAnArraySolution.FisherYatesDynamicArray.Shuffle(copy, new Random(1));

        Assert.Equal(original, copy);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FisherYatesDynamicArrayShuffle_AnyOriginal_ReturnsAPermutationOfTheOriginalElements(int[] original)
    {
        var shuffled = ShuffleAnArraySolution.FisherYatesDynamicArray.Shuffle(original, new Random(1));

        Assert.Equal(original.Length, shuffled.Length);
        Assert.Equal(original.OrderBy(x => x), shuffled.OrderBy(x => x));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByRemoveRandomRemaining_Shuffler_ShufflesToPermutationsAndResetsToNums(int[] nums)
    {
        var shuffler = ShuffleAnArraySolution.CreateByRemoveRandomRemaining(nums);

        AssertShufflesAndResets(shuffler, nums);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByFisherYatesDynamicArray_Shuffler_ShufflesToPermutationsAndResetsToNums(int[] nums)
    {
        var shuffler = ShuffleAnArraySolution.CreateByFisherYatesDynamicArray(nums);

        AssertShufflesAndResets(shuffler, nums);
    }

    [Fact]
    public void CreateByRemoveRandomRemaining_Shuffler_DrawsMoreThanOnePermutation()
    {
        var shuffler = ShuffleAnArraySolution.CreateByRemoveRandomRemaining([1, 2, 3, 4, 5]);

        var distinct = DistinctShufflesOf(shuffler);
        Assert.True(distinct > 1);
    }

    [Fact]
    public void CreateByFisherYatesDynamicArray_Shuffler_DrawsMoreThanOnePermutation()
    {
        var shuffler = ShuffleAnArraySolution.CreateByFisherYatesDynamicArray([1, 2, 3, 4, 5]);

        var distinct = DistinctShufflesOf(shuffler);
        Assert.True(distinct > 1);
    }

    // Every shuffle a permutation of nums, and every reset nums as given, even after the
    // caller has rewritten an array a shuffle handed back.
    private static void AssertShufflesAndResets(ShuffleAnArraySolution.IArrayShuffler shuffler, int[] nums)
    {
        var given = (int[])nums.Clone();

        for (var round = 0; round < Shuffles; round++)
        {
            var shuffled = shuffler.Shuffle();
            Assert.Equal(given.Order(), shuffled.Order());

            Array.Clear(shuffled);
            var reset = shuffler.Reset();
            Assert.Equal(given, reset);
        }
    }

    private static int DistinctShufflesOf(ShuffleAnArraySolution.IArrayShuffler shuffler)
    {
        var seen = new HashSet<string>();

        for (var round = 0; round < Shuffles; round++)
        {
            var shuffled = shuffler.Shuffle();
            var text = string.Join(',', shuffled);
            seen.Add(text);
        }

        return seen.Count;
    }
}
