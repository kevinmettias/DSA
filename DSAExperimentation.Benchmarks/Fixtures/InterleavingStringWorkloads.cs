namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 97: two seeded sources of the same length over a
// two-letter alphabet, and a target that is a seeded interleaving of them, so the
// answer is true by construction. Two letters rather than LC's 26 make the sources
// agree with the target's next letter often, so a search that takes the wrong
// source keeps going for a while before it fails instead of failing at once - the
// ambiguity the problem's dynamic programming exists for. FromFirst is the
// interleaving itself, one entry per target letter saying which source supplied it.
internal static class InterleavingStringWorkloads
{
    private const string Alphabet = "ab";
    private const int SourceCount = 2;

    public static (string First, string Second, string Target, bool[] FromFirst) Build(int sourceLength, Random random)
    {
        var first = Letters(sourceLength, random);
        var second = Letters(sourceLength, random);
        var firstPositions = SeededSequences.ShuffledZeroTo(SourceCount * sourceLength, random).Take(sourceLength).ToHashSet();
        var fromFirst = Enumerable.Range(0, SourceCount * sourceLength).Select(firstPositions.Contains).ToArray();
        var target = new char[fromFirst.Length];
        var (nextFirst, nextSecond) = (0, 0);

        for (var i = 0; i < target.Length; i++)
        {
            target[i] = fromFirst[i] ? first[nextFirst++] : second[nextSecond++];
        }

        return (first, second, new string(target), fromFirst);
    }

    private static string Letters(int length, Random random) =>
        new([.. SeededDraws.Values(length, 0, Alphabet.Length, random).Select(index => Alphabet[index])]);
}
