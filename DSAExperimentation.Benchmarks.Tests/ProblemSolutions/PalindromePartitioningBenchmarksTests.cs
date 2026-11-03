using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningBenchmarks (ARCHITECTURE 17.9). The class carries two
// arms - the backtracking walk and the precomputed-table walk - whose workload is a string drawn
// from seeded Random(131) over "ab". The expected partitions are derived here from a rebuilt string
// by brute force: at the smallest length every one of the 2^(n-1) ways to cut it is tried, and those
// whose pieces all read the same backwards are kept. The baseline is checked against that set - the
// walk emits it in its own order, which LeetCode leaves free - an expected value the generic check
// cannot supply; it holds the precomputed-table arm to the baseline.
public sealed partial class PalindromePartitioningBenchmarksTests
{
    private const int SmallestLength = 4;
    private const int RandomSeed = 131;
    private const string Alphabet = "ab";

    [Fact]
    public void Backtracking_SeededText_ReturnsEveryPalindromePartition() =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(BruteForcePartitions(RebuildText())),
            AnswerGraphText.OfUnordered(BuildHarness().Backtracking()));

    private static PalindromePartitioningBenchmarks BuildHarness()
    {
        var harness = new PalindromePartitioningBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The benchmark's own draw, rebuilt from its documented shape.
    private static string RebuildText() =>
        new([.. SeededDraws.Values(SmallestLength, 0, Alphabet.Length, new Random(RandomSeed)).Select(index => Alphabet[index])]);

    // Bit i of a cut set cuts the text after its letter i, so every partition is one cut set.
    private static List<List<string>> BruteForcePartitions(string text) =>
        [.. Enumerable.Range(0, 1 << (text.Length - 1))
            .Select(cuts => PiecesOf(text, cuts))
            .Where(pieces => pieces.All(piece => piece.SequenceEqual(piece.Reverse())))];

    private static List<string> PiecesOf(string text, int cuts)
    {
        var pieces = new List<string>();
        var start = 0;

        for (var letter = 0; letter < text.Length; letter++)
        {
            if (letter == text.Length - 1 || ((cuts >> letter) & 1) == 1)
            {
                pieces.Add(text[start..(letter + 1)]);
                start = letter + 1;
            }
        }

        return pieces;
    }
}
