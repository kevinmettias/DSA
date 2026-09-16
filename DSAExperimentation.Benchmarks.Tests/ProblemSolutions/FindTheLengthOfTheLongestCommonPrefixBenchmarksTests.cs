using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheLengthOfTheLongestCommonPrefixBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - comparing every arr1/arr2 pair digit by digit
// against walking arr2 through a digit trie built from arr1 - so a harness whose arms disagree is
// timing two different problems. Setup draws both arrays from one fixed seed and builds the trie
// from arr1, so the same Length must rebuild the same three values. The trie is read only
// (HasPrefix), so one harness is safe to call twice in either order.
public sealed partial class FindTheLengthOfTheLongestCommonPrefixBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithTrie()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Trie(), harness.BruteForce());
    }

    [Fact]
    public void Trie_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.Trie());
    }

    private static FindTheLengthOfTheLongestCommonPrefixBenchmarks BuildHarness()
    {
        var harness = new FindTheLengthOfTheLongestCommonPrefixBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
