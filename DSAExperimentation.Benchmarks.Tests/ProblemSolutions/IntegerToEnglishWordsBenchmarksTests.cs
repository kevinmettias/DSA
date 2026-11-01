using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IntegerToEnglishWordsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on the smallest Number, so this pins only that number's
// spelling, which LC 273's own wording fixes independently of either arm. This class has no
// [GlobalSetup]: Number is the whole input, so the harness is constructed with the smaller of the
// two [Params] values, a bare single-group number.
public sealed partial class IntegerToEnglishWordsBenchmarksTests
{
    private const int SmallestNumber = 123;

    // One hundred and twenty-three in LC 273's own words.
    private const string ExpectedSpellingForSmallestNumber = "One Hundred Twenty Three";

    [Fact]
    public void StringPrepend_SpelledNumber_ReturnsLeetCodesWording() =>
        Assert.Equal(ExpectedSpellingForSmallestNumber, BuildHarness().StringPrepend());

    private static IntegerToEnglishWordsBenchmarks BuildHarness() =>
        new() { Number = SmallestNumber };
}
