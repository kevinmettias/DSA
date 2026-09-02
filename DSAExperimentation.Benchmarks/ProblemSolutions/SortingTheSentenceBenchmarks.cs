using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sorting the Sentence (LC 1859), generalized beyond its real 1-9-word/single-digit
// constraint (the same "scale past the strict LeetCode bound to exercise real
// complexity" convention AddTwoNumbersBenchmarks/RelativeSortArrayBenchmarks already
// use) so a numeric position suffix of any length replaces the single trailing digit.
// BruteForce rescans the whole word array once per target position to place it -
// O(n^2). MergeSortByPosition instead sorts the words in place by their embedded
// position via this repo's own MergeSort.Sort<Element,TSequence> over an
// ArrayIndexedSequence with a custom comparer - O(n log n) - the same
// custom-comparer-over-ArrayIndexedSequence shape RelativeSortArrayBenchmarks uses.
[MemoryDiagnoser]
public class SortingTheSentenceBenchmarks
{
    private const int RandomSeed = 1859; // LC problem number

    [Params(200, 2_000)]
    public int Length;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var positions = Enumerable.Range(0, Length).OrderBy(_ => random.Next()).ToArray();
        _words = positions.Select(position => $"word{position}").ToArray();
    }

    [Benchmark(Baseline = true)]
    public string BruteForce()
    {
        var result = new string[_words.Length];

        for (var position = 0; position < _words.Length; position++)
        {
            foreach (var word in _words)
            {
                if (ExtractPosition(word) == position)
                {
                    result[position] = StripPosition(word);
                    break;
                }
            }
        }

        return string.Join(' ', result);
    }

    [Benchmark]
    public string MergeSortByPosition()
    {
        var words = (string[])_words.Clone();
        var byPosition = Comparer<string>.Create((a, b) => ExtractPosition(a).CompareTo(ExtractPosition(b)));

        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(words), byPosition);

        return string.Join(' ', words.Select(StripPosition));
    }

    private static int ExtractPosition(string word) => int.Parse(word.AsSpan(DigitsStart(word)));

    private static string StripPosition(string word) => word[..DigitsStart(word)];

    private static int DigitsStart(string word)
    {
        var digitsStart = word.Length;
        while (digitsStart > 0 && char.IsAsciiDigit(word[digitsStart - 1]))
        {
            digitsStart--;
        }

        return digitsStart;
    }
}
