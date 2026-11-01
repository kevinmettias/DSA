using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ExtraCharactersInAStringBenchmarks (ARCHITECTURE 17.9): an expected count
// known from the workload's shape. Setup builds the text from 'k'..'o' while every dictionary word is
// built from 'a'..'j', so no word can cover a single character; that is what forces both arms through
// their full per-start scan instead of an early exact match, and it makes the answer decisive: every
// character of the text is an extra character.
public sealed partial class ExtraCharactersInAStringBenchmarksTests
{
    private const int SmallestLength = 10;

    // Setup's characters never appear in the dictionary, so no word can cover any of them and the
    // minimum extra characters is the whole length.
    private const int ExpectedExtraCharacterCount = SmallestLength;

    [Fact]
    public void HashSetFullScan_UndictionaryableText_CountsEveryCharacterAsExtra() =>
        Assert.Equal(ExpectedExtraCharacterCount, BuildHarness().HashSetFullScan());

    private static ExtraCharactersInAStringBenchmarks BuildHarness()
    {
        var harness = new ExtraCharactersInAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
