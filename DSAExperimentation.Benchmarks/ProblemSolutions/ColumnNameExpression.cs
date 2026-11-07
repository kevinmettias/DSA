using System.Text;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The input BasicCalculatorIVBenchmarks' arms are fed: a sum of `length` distinct base-26
// "spreadsheet column" variable names (a, b, ..., z, aa, ab, ...) joined by " + ", since LC 770
// separates every token by a single space. Every name it
// emits is valid per LC 770's lowercase-letters-only variable grammar however large the
// requested length grows, so the one generator serves every Length row the harness declares.
// Pure and stateless: an int in, a string out, sharing nothing with the arms that consume it.
internal static class ColumnNameExpression
{
    private const string Plus = " + ";

    internal static string Build(int length)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            if (i > 0)
            {
                builder.Append(Plus);
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
            chars.Insert(0, (char)('a' + (n % EnglishLetters.Size)));
            n = (n / EnglishLetters.Size) - 1;
        } while (n >= 0);

        return new string(chars.ToArray());
    }
}
