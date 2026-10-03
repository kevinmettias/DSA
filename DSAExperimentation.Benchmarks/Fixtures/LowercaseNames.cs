using System.Text;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Distinct names spelled in lowercase letters only, for the workloads whose problems
// name things - foods, cuisines, tables, tokens - and promise those names are
// lowercase English letters. A name is its index written in bijective base 26 (a, b,
// ..., z, aa, ab, ...), so distinct indices get distinct names and every name is as
// short as the count allows: 26 names of one letter, then 676 of two, then 17,576 of
// three. OfWidth serves the problems that fix a name's length instead.
internal static class LowercaseNames
{
    private const int AlphabetSize = 26;

    public static string Of(int index)
    {
        var letters = new StringBuilder();

        for (var remaining = index + 1; remaining > 0; remaining = (remaining - 1) / AlphabetSize)
        {
            var letter = (char)('a' + ((remaining - 1) % AlphabetSize));
            letters.Insert(0, letter);
        }

        return letters.ToString();
    }

    // The index written in plain base 26 (a is 0) across exactly width letters, so
    // distinct indices below 26^width get distinct names of that one length.
    public static string OfWidth(int index, int width)
    {
        var letters = new char[width];
        var remaining = index;

        for (var position = width - 1; position >= 0; position--)
        {
            letters[position] = (char)('a' + (remaining % AlphabetSize));
            remaining /= AlphabetSize;
        }

        return new string(letters);
    }
}
