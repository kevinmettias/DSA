namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 205: a guaranteed-isomorphic pair of
// strings so the checker never short-circuits on an early mismatch and both
// strategies scan the full requested length. The target string is derived
// from the source by a random single-letter substitution cipher - a
// bijection over 'a'..'z' - which is exactly what "isomorphic" requires.
internal static class IsomorphicStringWorkloads
{
    private const int AlphabetSize = 26;

    public static (string Source, string Target) BuildIsomorphicPair(int length, int seed)
    {
        var random = new Random(seed);
        var cipher = BuildRandomLetterPermutation(random);

        var source = new char[length];
        var target = new char[length];

        for (var i = 0; i < length; i++)
        {
            var letter = (char)('a' + random.Next(AlphabetSize));
            source[i] = letter;
            target[i] = cipher[letter - 'a'];
        }

        return (new string(source), new string(target));
    }

    private static char[] BuildRandomLetterPermutation(Random random)
    {
        var letters = new char[AlphabetSize];

        for (var i = 0; i < AlphabetSize; i++)
        {
            letters[i] = (char)('a' + i);
        }

        for (var i = letters.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (letters[i], letters[j]) = (letters[j], letters[i]);
        }

        return letters;
    }
}
