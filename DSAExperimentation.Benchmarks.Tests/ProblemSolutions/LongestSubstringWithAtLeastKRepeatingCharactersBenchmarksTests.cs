using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestSubstringWithAtLeastKRepeatingCharactersBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - scoring every substring
// against a fresh count table against recursing on the characters that fall short of K - so a
// harness whose arms disagree is timing two different problems. Both arms return the longest
// length, a scalar compared directly. Setup builds the text from a fixed seed through
// LongestSubstringWithAtLeastKRepeatingCharactersWorkloads, so the same Length must rebuild the
// same string; the length itself depends on those draws, so the arms are compared against each
// other alone.
public sealed partial class LongestSubstringWithAtLeastKRepeatingCharactersBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().DivideAndConquerHashMap(),
            BuildHarness().DivideAndConquerHashMap());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithDivideAndConquerHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DivideAndConquerHashMap(), harness.BruteForce());
    }

    [Fact]
    public void DivideAndConquerHashMap_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.DivideAndConquerHashMap());
    }

    private static LongestSubstringWithAtLeastKRepeatingCharactersBenchmarks BuildHarness()
    {
        var harness = new LongestSubstringWithAtLeastKRepeatingCharactersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
