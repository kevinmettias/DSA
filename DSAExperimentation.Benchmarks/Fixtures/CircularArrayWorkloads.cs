namespace DSAExperimentation.Benchmarks.Fixtures;

// Sizes a Shortest Distance to Target String in a Circular Array scenario (LC
// 2515): length filler words arranged in a circle, with exactly one word replaced
// by the target string at the index diametrically opposite startIndex (0) - the
// worst case for either search direction, forcing both benchmarked approaches
// through real work instead of an immediate first-neighbor match.
//
// Workload shape only (ARCHITECTURE.md #17.7). Turning these words into the graph
// the BFS arm walks is CircularArrayGraph.Build's job, one tier down.
internal static class CircularArrayWorkloads
{
    public const string Target = "target-word";
    public const int StartIndex = 0;
    private const string Filler = "filler-word";

    public static string[] BuildWords(int length)
    {
        var words = new string[length];

        for (var i = 0; i < length; i++)
        {
            words[i] = Filler;
        }

        words[length / 2] = Target;

        return words;
    }
}
