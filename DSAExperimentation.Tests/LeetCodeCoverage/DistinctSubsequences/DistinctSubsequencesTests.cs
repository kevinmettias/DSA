using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistinctSubsequences;

public sealed partial class DistinctSubsequencesTests
{
    [Theory]
    [InlineData("rabbbit", "rabbit", 3)]
    [InlineData("babgbag", "bag", 5)]
    public void NumDistinct_LeetCodeExamples_ReturnsCount(string source, string target, int expected) => Assert.Equal(expected, Count(source, target));
    private static int Count(string source, string target) { return Memoizer.Memoize<(int Source, int Target), int>((0, 0), Ways); int Ways((int Source, int Target) state, Func<(int Source, int Target), int> ways) { var (i, j) = state; if (j == target.Length) return 1; if (i == source.Length) return 0; var total = ways((i + 1, j)); if (source[i] == target[j]) total += ways((i + 1, j + 1)); return total; } }
}
