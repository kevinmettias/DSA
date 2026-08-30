using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Print Words Vertically (LC 1324): building each column with a plain
// List<char> and trimming trailing spaces via a materialized-string TrimEnd
// pass (baseline) vs. this repo's own DynamicArray<char>
// (StreamOfCharactersBenchmarks' own precedent for a growable char buffer),
// trimming trailing spaces in place by popping from the tail with
// RemoveAt(Count - 1) instead of allocating a second trimmed string.
[MemoryDiagnoser]
public class PrintWordsVerticallyBenchmarks
{
    [Params(50, 500)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1324);
        _words = [.. Enumerable.Range(0, WordCount).Select(_ => RandomWord(random))];
    }

    private static string RandomWord(Random random)
        => new([.. Enumerable.Range(0, random.Next(1, 12)).Select(_ => (char)('A' + random.Next(26)))]);

    [Benchmark(Baseline = true)]
    public int ListCharTrimEnd()
    {
        var maxLength = _words.Max(word => word.Length);
        var rows = 0;

        for (var column = 0; column < maxLength; column++)
        {
            var chars = new List<char>();

            foreach (var word in _words)
            {
                chars.Add(column < word.Length ? word[column] : ' ');
            }

            var row = new string([.. chars]).TrimEnd(' ');
            rows += row.Length;
        }

        return rows;
    }

    [Benchmark]
    public int DynamicArrayTrimTail()
    {
        var maxLength = _words.Max(word => word.Length);
        var rows = 0;

        for (var column = 0; column < maxLength; column++)
        {
            var buffer = new DynamicArray<char>();

            foreach (var word in _words)
            {
                buffer.Add(column < word.Length ? word[column] : ' ');
            }

            while (buffer.Count > 0 && buffer.Get(buffer.Count - 1) == ' ')
            {
                buffer.RemoveAt(buffer.Count - 1);
            }

            rows += buffer.Count;
        }

        return rows;
    }
}
