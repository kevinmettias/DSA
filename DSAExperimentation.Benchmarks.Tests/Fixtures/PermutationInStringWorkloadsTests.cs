using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PermutationInStringWorkloads (ARCHITECTURE 17.7). The reading depends on LC 567's
// haystack being drawn from an alphabet that excludes every letter of the benchmark's fixed pattern
// "aeiou", so neither strategy ever finds a match and both are forced through their full worst-case scan.
public sealed partial class PermutationInStringWorkloadsTests
{
    private const int Length = 2_000;
    private const int Seed = 567; // LC problem number
    private const string Alphabet = "bcdfghjklmnpqrstvwxyz";
    private const string Pattern = "aeiou"; // the pattern PermutationInStringBenchmarks fixes

    [Fact]
    public void BuildHaystack_Length_ReturnsAStringOfExactlyThatLength() =>
        Assert.Equal(Length, PermutationInStringWorkloads.BuildHaystack(Length, Seed).Length);

    [Fact]
    public void BuildHaystack_EveryCharacter_ComesFromTheDocumentedConsonantAlphabet() =>
        Assert.All(PermutationInStringWorkloads.BuildHaystack(Length, Seed), letter => Assert.Contains(letter, Alphabet));

    // The exclusion is what makes the benchmark a worst case rather than a search that stops early: a
    // single pattern letter in the haystack would let either arm return a match before its full scan.
    [Fact]
    public void BuildHaystack_Haystack_ContainsNoLetterOfTheBenchmarksFixedPattern() =>
        Assert.DoesNotContain(PermutationInStringWorkloads.BuildHaystack(Length, Seed), Pattern.Contains);

    [Fact]
    public void BuildHaystack_SameSeed_ReturnsTheSameHaystack() =>
        Assert.Equal(
            PermutationInStringWorkloads.BuildHaystack(Length, Seed),
            PermutationInStringWorkloads.BuildHaystack(Length, Seed));
}
