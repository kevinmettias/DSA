using System.Text;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 394 - an encoded string of at least the
// requested Length, built from many *sequential*, only single-level-nested
// "2[ab]" groups so recursion depth stays constant as Length grows instead of
// risking a StackOverflowException in the recursive-descent baseline - the
// same shape BasicCalculatorWorkloads already uses.
internal static class DecodeStringWorkloads
{
    private const string EncodedTile = "2[ab]";

    public static string BuildEncoded(int length)
    {
        var builder = new StringBuilder();

        while (builder.Length < length)
        {
            builder.Append(EncodedTile);
        }

        return builder.ToString();
    }
}
