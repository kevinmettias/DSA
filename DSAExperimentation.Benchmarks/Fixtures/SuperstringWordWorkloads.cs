namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 943 - how many words to order and how long each
// one is. Distinct equal-length words keep the problem's own "no word is a substring
// of another" premise true for free, and a DNA-sized alphabet makes genuine
// suffix/prefix overlaps common enough that the ordering search has real structure to
// find rather than a matrix of zeroes.
internal static class SuperstringWordWorkloads
{
    private const string Alphabet = "ACGT";

    public static string[] BuildWords(int count, int wordLength, int seed)
    {
        var random = new Random(seed);
        var seen = new HashSet<string>();
        var words = new List<string>();

        while (words.Count < count)
        {
            var word = BuildWord(random, wordLength);

            if (seen.Add(word))
            {
                words.Add(word);
            }
        }

        return [.. words];
    }

    private static string BuildWord(Random random, int wordLength)
    {
        var bases = new char[wordLength];

        for (var i = 0; i < wordLength; i++)
        {
            bases[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        return new string(bases);
    }
}
