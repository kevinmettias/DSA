using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfMovesToMakePalindromeBenchmarks (ARCHITECTURE 17.9): both
// arms are MinimumNumberOfMovesToMakePalindromeSolution's, the same methods
// MinimumNumberOfMovesToMakePalindromeTests proves correct, and both return the fewest adjacent
// swaps that make the string a palindrome. They run the same two-pointer greedy over different
// mutable buffers - a BCL List<char> against this repo's ArrayIndexedSequence<char> - so arms
// that disagree are timing two different problems.
//
// Both arms copy the string into their own buffer and never write back to the harness, so one
// harness instance answers both arms and the comparison is a genuine same-input comparison.
public sealed partial class MinimumNumberOfMovesToMakePalindromeBenchmarksTests
{
    // The smallest declared [Params] value: both arms are quadratic in the string's length, and
    // Setup still builds a fully shuffled, palindrome-ready string at this size.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().ListRemoveInsert(),
            BuildHarness().ListRemoveInsert());

    [Fact]
    public void ListRemoveInsert_AgreesWithArrayIndexedSequenceSwap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayIndexedSequenceSwap(), harness.ListRemoveInsert());
    }

    [Fact]
    public void ArrayIndexedSequenceSwap_AgreesWithListRemoveInsert()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListRemoveInsert(), harness.ArrayIndexedSequenceSwap());
    }

    private static MinimumNumberOfMovesToMakePalindromeBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfMovesToMakePalindromeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
