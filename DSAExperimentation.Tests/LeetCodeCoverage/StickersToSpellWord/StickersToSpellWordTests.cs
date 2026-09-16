using DSAExperimentation.LeetCode.StickersToSpellWord;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StickersToSpellWord;

// Harness only. Both recursion strategies live in StickersToSpellWordSolution and
// are asserted against the same examples, including the case where the target
// letters can never all be covered.
public sealed partial class StickersToSpellWordTests
{
    public static TheoryData<string[], string, int> Examples =>
        new()
        {
            { ["with", "example", "science"], "thehat", 3 },
            { ["notice", "possible"], "basicbasic", -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinStickersByNaiveRecursion_LeetCodeExamples_ReturnsFewestStickers(
        string[] stickers, string target, int expected)
    {
        var actual = StickersToSpellWordSolution.MinStickersByNaiveRecursion(stickers, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinStickersByMemoizedRecursion_LeetCodeExamples_ReturnsFewestStickers(
        string[] stickers, string target, int expected)
    {
        var actual = StickersToSpellWordSolution.MinStickersByMemoizedRecursion(stickers, target);

        Assert.Equal(expected, actual);
    }
}
