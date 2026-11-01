using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningIIBenchmarks (ARCHITECTURE 17.9): the baseline's minimum
// against one derived independently of the arms, which the generic check cannot supply - it holds the
// memoized suffix recurrence to the baseline. Setup's string is fully documented (seeded
// Random(132), letters drawn over "ab"), so the expected minimum is derived here from a rebuilt string
// by the plainest recurrence there is: the fewest palindromes any prefix splits into, tried over every
// last piece and checked letter by letter. That shares neither arm's suffix memo nor its centre
// expansion.
public sealed partial class PalindromePartitioningIIBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int RandomSeed = 132;
    private const string Alphabet = "ab";

    [Fact]
    public void IterativeDynamicProgramming_SeededText_ReturnsTheMinimumCut() =>
        Assert.Equal(IndependentMinCut(RebuildText()), BuildHarness().IterativeDynamicProgramming());

    private static PalindromePartitioningIIBenchmarks BuildHarness()
    {
        var harness = new PalindromePartitioningIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The benchmark's own draw, rebuilt from its documented shape.
    private static string RebuildText() =>
        new([.. SeededDraws.Values(SmallestLength, 0, Alphabet.Length, new Random(RandomSeed)).Select(index => Alphabet[index])]);

    // fewestPieces[end] is the fewest palindromes text[..end] splits into; a split into k pieces makes
    // k - 1 cuts.
    private static int IndependentMinCut(string text)
    {
        var fewestPieces = new int[text.Length + 1];

        for (var end = 1; end <= text.Length; end++)
        {
            fewestPieces[end] = Enumerable.Range(0, end)
                .Where(start => IsPalindrome(text[start..end]))
                .Min(start => fewestPieces[start] + 1);
        }

        return fewestPieces[text.Length] - 1;
    }

    private static bool IsPalindrome(string piece) => piece.SequenceEqual(piece.Reverse());
}
