using DSAExperimentation.LeetCode.ShuffleAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShuffleAnArray;

// Harness only: both strategies live in ShuffleAnArraySolution. LeetCode's own
// reset() is the identity on the caller's stored array, so instead of a dedicated
// reset() method this asserts the real invariant reset() depends on - shuffling
// never mutates the array it was given - directly against each strategy.
public sealed class ShuffleAnArrayTests
{
    public static TheoryData<int[]> Examples =>
        new()
        {
            { [1, 2, 3] },
            { [1, 2, 3, 4, 5] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShuffleByRemoveRandomRemaining_AfterShuffling_OriginalIsUnchanged(int[] original)
    {
        var copy = (int[])original.Clone();

        ShuffleAnArraySolution.ShuffleByRemoveRandomRemaining(copy, new Random(1));

        Assert.Equal(original, copy);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShuffleByRemoveRandomRemaining_ReturnsAPermutationOfTheOriginalElements(int[] original)
    {
        var shuffled = ShuffleAnArraySolution.ShuffleByRemoveRandomRemaining(original, new Random(1));

        Assert.Equal(original.Length, shuffled.Length);
        Assert.Equal(original.OrderBy(x => x), shuffled.OrderBy(x => x));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShuffleByFisherYatesDynamicArray_AfterShuffling_OriginalIsUnchanged(int[] original)
    {
        var copy = (int[])original.Clone();

        ShuffleAnArraySolution.ShuffleByFisherYatesDynamicArray(copy, new Random(1));

        Assert.Equal(original, copy);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShuffleByFisherYatesDynamicArray_ReturnsAPermutationOfTheOriginalElements(int[] original)
    {
        var shuffled = ShuffleAnArraySolution.ShuffleByFisherYatesDynamicArray(original, new Random(1));

        Assert.Equal(original.Length, shuffled.Length);
        Assert.Equal(original.OrderBy(x => x), shuffled.OrderBy(x => x));
    }
}
