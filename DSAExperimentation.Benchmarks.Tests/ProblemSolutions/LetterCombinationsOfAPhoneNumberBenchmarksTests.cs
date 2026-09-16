using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LetterCombinationsOfAPhoneNumberBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - nested list expansion carried by hand against this
// repo's generic Backtrack.Search - so a harness whose arms disagree is timing two different problems.
// Both build the Cartesian product in phone-keypad order, an outer order the problem itself pins, so
// the two lists are rendered positionally rather than as an unordered set.
//
// Setup builds the digit string from one repeated digit, so the same DigitCount must rebuild the same
// string; otherwise two published numbers were never comparable in the first place.
public sealed partial class LetterCombinationsOfAPhoneNumberBenchmarksTests
{
    private const int SmallestDigitCount = 3;

    [Fact]
    public void Setup_SameDigitCount_RebuildsTheSameDigits() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().IterativeExpansion()),
            AnswerText.Of(BuildHarness().IterativeExpansion()));

    [Fact]
    public void IterativeExpansion_RepeatedKeypadDigits_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.Backtracking()), AnswerText.Of(harness.IterativeExpansion()));
    }

    [Fact]
    public void Backtracking_RepeatedKeypadDigits_AgreesWithIterativeExpansion()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.IterativeExpansion()), AnswerText.Of(harness.Backtracking()));
    }

    private static LetterCombinationsOfAPhoneNumberBenchmarks BuildHarness()
    {
        var harness = new LetterCombinationsOfAPhoneNumberBenchmarks { DigitCount = SmallestDigitCount };
        harness.Setup();

        return harness;
    }
}
