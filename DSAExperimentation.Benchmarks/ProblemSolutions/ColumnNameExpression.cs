using System.Text;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The input BasicCalculatorIVBenchmarks' arms are fed: a sum of `length` distinct base-26
// "spreadsheet column" variable names (a, b, ..., z, aa, ab, ...) joined by '+'. Every name it
// emits is valid per LC 770's lowercase-letters-only variable grammar however large the
// requested length grows, so the one generator serves every Length row the harness declares.
// Pure and stateless: an int in, a string out, sharing nothing with the arms that consume it.
internal static class ColumnNameExpression
{
    // Base-26 "spreadsheet column" alphabet size (a-z).
    private const int AlphabetSize = 26;

    internal static string Build(int length)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            if (i > 0)
            {
                builder.Append('+');
            }

            builder.Append(VariableName(i));
        }

        return builder.ToString();
    }

    // Base-26 "spreadsheet column" naming (a, b, ..., z, aa, ab, ...) so every
    // generated name is valid per LC 770's lowercase-letters-only variable grammar,
    // however large the requested length grows.
    private static string VariableName(int index)
    {
        var chars = new List<char>();
        var n = index;

        do
        {
            chars.Insert(0, (char)('a' + (n % AlphabetSize)));
            n = (n / AlphabetSize) - 1;
        } while (n >= 0);

        return new string(chars.ToArray());
    }
}
