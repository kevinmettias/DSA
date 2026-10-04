using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignATextEditorBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins what every report must be, from the script alone.
//
// Iteration i adds a chunk whose first letter is the alphabet's (i mod 26)-th, then walks the
// cursor back over the other four, so exactly that first letter joins the text left of the
// cursor. After iteration i the left text is those first letters in order, and LeetCode 2296
// reports its last min(10, length) characters.
public sealed partial class DesignATextEditorBenchmarksTests
{
    private const int SmallestOperationCount = 200;
    private const int ReportedWindow = 10;
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

    [Fact]
    public void ListBacked_ChunksWalkedBackToTheirFirstLetter_ReportsTheLastTenFirstLetters() =>
        Assert.Equal(ExpectedReports(), BuildHarness().ListBacked());

    [Fact]
    public void StackBacked_ChunksWalkedBackToTheirFirstLetter_ReportsTheLastTenFirstLetters() =>
        Assert.Equal(ExpectedReports(), BuildHarness().StackBacked());

    private static string[] ExpectedReports()
    {
        var reports = new string[SmallestOperationCount];
        var left = string.Empty;

        for (var i = 0; i < SmallestOperationCount; i++)
        {
            left += Alphabet[i % Alphabet.Length];
            reports[i] = left[Math.Max(0, left.Length - ReportedWindow)..];
        }

        return reports;
    }

    private static DesignATextEditorBenchmarks BuildHarness()
    {
        var harness = new DesignATextEditorBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
