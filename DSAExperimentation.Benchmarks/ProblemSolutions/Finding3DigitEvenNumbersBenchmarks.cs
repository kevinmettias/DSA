using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Finding 3-Digit Even Numbers (LC 2094): both strategies enumerate the same O(n^3)
// triple of distinct array positions - that part of the work is inherent to the
// problem (a candidate 3-digit number is defined by which three positions were
// picked, not just which three values). What differs is how each candidate is
// deduped and how the final list is sorted. ListContainsScanDedupe checks
// membership with List<int>.Contains - an O(found-so-far) linear scan repeated for
// every one of the O(n^3) candidates - then sorts with List<T>.Sort. SetDedupeThenMergeSort
// instead uses this repo's own Set<int> for an O(1) expected membership check (the
// same "TryAdd guards a growing result list" shape the coverage test already uses)
// and this repo's MergeSort over ArrayIndexedSequence for the final ascending order.
// Digits are drawn from a small alphabet (0-9) specifically so most of the ~450
// possible distinct 3-digit even numbers actually get found, giving the baseline's
// growing List.Contains scan real duplicate-checking work to pay for on every
// candidate instead of exiting a mostly-empty list immediately.
[MemoryDiagnoser]
public class Finding3DigitEvenNumbersBenchmarks
{
    private const int RandomSeed = 2094; // LC problem number
    private const int DigitRangeExclusive = 10;
    private const int OddEvenDivisor = 2;
    private const int HundredsPlace = 100;
    private const int TensPlace = 10;

    [Params(30, 100)]
    public int Length;

    private int[] _digits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _digits = Enumerable.Range(0, Length).Select(_ => random.Next(0, DigitRangeExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] ListContainsScanDedupe()
    {
        var found = new List<int>();

        ForEachCandidateNumber(number =>
        {
            if (!found.Contains(number))
            {
                found.Add(number);
            }
        });

        found.Sort();
        return found.ToArray();
    }

    [Benchmark]
    public int[] SetDedupeThenMergeSort()
    {
        var seen = new Set<int>();
        var found = new List<int>();

        ForEachCandidateNumber(number =>
        {
            if (seen.TryAdd(number))
            {
                found.Add(number);
            }
        });

        var result = found.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(result));
        return result;
    }

    private void ForEachCandidateNumber(Action<int> onCandidate)
    {
        for (var hundreds = 0; hundreds < _digits.Length; hundreds++)
        {
            if (_digits[hundreds] == 0)
            {
                continue;
            }

            for (var tens = 0; tens < _digits.Length; tens++)
            {
                if (tens == hundreds)
                {
                    continue;
                }

                EmitCandidatesForPair(hundreds, tens, onCandidate);
            }
        }
    }

    private void EmitCandidatesForPair(int hundreds, int tens, Action<int> onCandidate)
    {
        for (var ones = 0; ones < _digits.Length; ones++)
        {
            if (ones == hundreds || ones == tens || _digits[ones] % OddEvenDivisor != 0)
            {
                continue;
            }

            var number = (_digits[hundreds] * HundredsPlace) + (_digits[tens] * TensPlace) + _digits[ones];
            onCandidate(number);
        }
    }
}
