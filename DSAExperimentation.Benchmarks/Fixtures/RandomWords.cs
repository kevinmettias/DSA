namespace DSAExperimentation.Benchmarks.Fixtures;

// Words of random length over a deliberately small alphabet - the workload three
// string-triple harnesses need (LC 3485's k-string prefix queries, LC 3042's
// prefix/suffix pairs, LC 3093's suffix queries). All three narrow the alphabet for
// the same reason: over the full 26 letters almost every candidate pair diverges on
// its first (or last) character, so no strategy gets to walk a meaningful span. Which
// alphabet, and how long a word may run, is each problem's own workload sizing, so
// both stay at the call site.
internal static class RandomWords
{
    public static string[] Build(int count, string alphabet, int maxLength, int seed) =>
        Build(count, alphabet, maxLength, new Random(seed));

    public static string[] Build(int count, string alphabet, int maxLength, Random random)
    {
        var words = new string[count];

        for (var i = 0; i < count; i++)
        {
            var length = random.Next(1, maxLength + 1);
            var chars = new char[length];

            for (var j = 0; j < length; j++)
            {
                chars[j] = alphabet[random.Next(alphabet.Length)];
            }

            words[i] = new string(chars);
        }

        return words;
    }
}
