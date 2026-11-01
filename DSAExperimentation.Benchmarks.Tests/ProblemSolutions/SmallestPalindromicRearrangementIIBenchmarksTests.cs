using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestPalindromicRearrangementIIBenchmarks (ARCHITECTURE 17.9). Arm
// agreement is BenchmarkArmsTests' job. The half's arrangement count stays well above the harness's
// own rank at both tunings, so an answer always exists and neither arm may report the problem's
// "no such arrangement" sentinel; and the question has a property checkable from the answer alone:
// the rearrangement is as long as the text it came from and is a palindrome.
public sealed partial class SmallestPalindromicRearrangementIIBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void BacktrackingRank_TwentyCharacterPalindrome_ReturnsAFullLengthPalindrome()
    {
        var answer = BuildHarness().BacktrackingRank();

        Assert.Equal(SmallestLength, answer.Length);
        Assert.Equal(answer, Reversed(answer));
    }

    private static string Reversed(string text) => new([.. text.Reverse()]);

    private static SmallestPalindromicRearrangementIIBenchmarks BuildHarness()
    {
        var harness = new SmallestPalindromicRearrangementIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
