using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestPalindromicRearrangementIBenchmarks (ARCHITECTURE 17.9). Arm agreement
// is BenchmarkArmsTests' job; the question has a property checkable from the answer alone: a
// rearrangement of the text is the same length as the text and is itself a palindrome, so a
// rearrangement that came back as a sorted half, or truncated, fails below even when the two arms
// fail identically.
public sealed partial class SmallestPalindromicRearrangementIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void CharArrayReverse_TwoHundredCharacterPalindrome_ReturnsAFullLengthPalindrome()
    {
        var answer = BuildHarness().CharArrayReverse();

        Assert.Equal(SmallestLength, answer.Length);
        Assert.Equal(answer, Reversed(answer));
    }

    private static string Reversed(string text) => new([.. text.Reverse()]);

    private static SmallestPalindromicRearrangementIBenchmarks BuildHarness()
    {
        var harness = new SmallestPalindromicRearrangementIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
