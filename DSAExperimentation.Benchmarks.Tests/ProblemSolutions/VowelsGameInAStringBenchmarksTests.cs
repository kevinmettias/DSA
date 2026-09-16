using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for VowelsGameInAStringBenchmarks (ARCHITECTURE 17.9): its two arms are
// VowelsGameInAStringSolution's competing strategies for the same question - the recursive game
// search over real positions against the closed form that reads the answer off the string's vowel
// count - so a harness whose arms disagree is answering two different questions about LC 3227.
//
// Setup draws Length characters off one seeded alphabet whose first five letters are vowels, and the
// drawn word contains at least one of them, so LC 3227's answer is that Alice wins. That decisive
// literal is asserted alongside the arms' agreement, which both arms are bool makes the only
// observable this harness has: the word the search explored is not reported anywhere.
public sealed partial class VowelsGameInAStringBenchmarksTests
{
    // The smaller of Setup's [Params(8, 20)] lengths.
    private const int SmallestLength = 8;

    // Setup's documented outcome: the seeded draw leaves a vowel in the word, so Alice can take it.
    private const bool ExpectedAliceWins = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().CanAliceWinByGameSearch(), BuildHarness().CanAliceWinByGameSearch());
        Assert.Equal(ExpectedAliceWins, BuildHarness().CanAliceWinByGameSearch());
    }

    [Fact]
    public void CanAliceWinByGameSearch_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanAliceWinByVowelExistence(), harness.CanAliceWinByGameSearch());
        Assert.Equal(ExpectedAliceWins, harness.CanAliceWinByGameSearch());
    }

    [Fact]
    public void CanAliceWinByVowelExistence_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanAliceWinByGameSearch(), harness.CanAliceWinByVowelExistence());
        Assert.Equal(ExpectedAliceWins, harness.CanAliceWinByVowelExistence());
    }

    private static VowelsGameInAStringBenchmarks BuildHarness()
    {
        var harness = new VowelsGameInAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
