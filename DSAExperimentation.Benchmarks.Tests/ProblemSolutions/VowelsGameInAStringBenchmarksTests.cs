using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for VowelsGameInAStringBenchmarks (ARCHITECTURE 17.9): the generic check holds
// the arms to agreeing with each other, which a shared wrong verdict would pass, so this pins the
// baseline's verdict to the one the drawn word fixes.
//
// Setup draws Length characters off one seeded alphabet whose first five letters are vowels, and the
// drawn word contains at least one of them, so LC 3227's answer is that Alice wins. Both arms return
// bool, so the verdict is the only observable this harness has: the word the search explored is not
// reported anywhere.
public sealed partial class VowelsGameInAStringBenchmarksTests
{
    // The smaller of Setup's [Params(8, 20)] lengths.
    private const int SmallestLength = 8;

    // Setup's documented outcome: the seeded draw leaves a vowel in the word, so Alice can take it.
    private const bool ExpectedAliceWins = true;

    [Fact]
    public void CanAliceWinByGameSearch_WordWithAVowel_ReportsAliceWins() =>
        Assert.Equal(ExpectedAliceWins, BuildHarness().CanAliceWinByGameSearch());

    private static VowelsGameInAStringBenchmarks BuildHarness()
    {
        var harness = new VowelsGameInAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
