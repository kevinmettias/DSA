using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find the Kth Largest Integer in the Array (LC 1985): digit strings compared numerically (by
// length, then ordinal) via NumericStringToken. FullSort clones and Array.Sorts every string with
// that comparer (O(n log n)) then indexes; SizeKMinHeap reuses this repo's own
// Heap<Element,MinHeapOrder<Element>> (KthLargestBenchmarks/LastStoneWeightBenchmarks precedent),
// discarding the smallest root once the heap grows past K so its own log factor is on K, not N.
[MemoryDiagnoser]
public class FindTheKthLargestIntegerInTheArrayBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 1985; // LC problem number
    private const int MaxDigitCountExclusive = 12;
    private const int NonZeroDigitChoices = 9; // leading digit is drawn from 1-9
    private const int DecimalDigitRadix = 10; // remaining digits are drawn from 0-9

    [Params(500, 10_000)]
    public int Length;

    private string[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => RandomDigitString(random)).ToArray();
    }

    private static string RandomDigitString(Random random)
    {
        var digitCount = random.Next(1, MaxDigitCountExclusive);
        var digits = new char[digitCount];
        digits[0] = (char)('1' + random.Next(NonZeroDigitChoices));

        for (var i = 1; i < digitCount; i++)
        {
            digits[i] = (char)('0' + random.Next(DecimalDigitRadix));
        }

        return new string(digits);
    }

    private static int CompareNumeric(string first, string second) => first.Length != second.Length
        ? first.Length.CompareTo(second.Length)
        : string.CompareOrdinal(first, second);

    [Benchmark(Baseline = true)]
    public string FullSort()
    {
        var copy = (string[])_values.Clone();
        Array.Sort(copy, CompareNumeric);
        return copy[^K];
    }

    [Benchmark]
    public string SizeKMinHeap()
    {
        var heap = new Heap<NumericStringToken, MinHeapOrder<NumericStringToken>>();

        foreach (var value in _values)
        {
            heap.Push(new NumericStringToken(value));

            if (heap.Count > K)
            {
                heap.TryPop(out _);
            }
        }

        heap.TryPeek(out var kthLargest);
        return kthLargest.Value;
    }

    private readonly record struct NumericStringToken(string Value) : IComparable<NumericStringToken>
    {
        public int CompareTo(NumericStringToken other) => CompareNumeric(Value, other.Value);
    }
}
