using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.Finding3DigitEvenNumbers;

// LeetCode 2094. Finding 3-Digit Even Numbers: every 3-digit even number that can
// be formed from three DISTINCT array positions (not distinct values - the same
// digit value at two different positions may both be used), with no leading zero,
// reported in ascending order with duplicates removed.
//
// Both strategies enumerate the same O(n^3) triple of positions, because that part
// of the work is inherent to the problem: a candidate is defined by which three
// positions were picked, not just which three values. What differs is how each
// candidate is deduped and how the final list is sorted.
internal static class Finding3DigitEvenNumbersSolution
{
    private const int HundredsPlace = 100;
    private const int TensPlace = 10;
    private const int OddEvenDivisor = 2;

    // The textbook answer: dedupe with List<int>.Contains - an O(found-so-far)
    // linear scan repeated for every one of the O(n^3) candidates - and sort with
    // List<T>.Sort. Deliberately written with BCL parts only; it is the arm the
    // composed strategy below has to justify itself against.
    public static int[] FindEvenNumbersByListScanDedupe(int[] digits)
    {
        var found = new List<int>();

        ForEachCandidateNumber(digits, new ListScanCollector(found));

        found.Sort();
        return found.ToArray();
    }

    // This repo's own parts: Set<int> for an O(1) expected membership check - the
    // same "TryAdd guards a growing result list" shape AccountsMerge uses for its
    // email dedupe - and MergeSort over ArrayIndexedSequence for the final
    // ascending order, the sorting convention SortAnArray already establishes.
    public static int[] FindEvenNumbersBySetDedupe(int[] digits)
    {
        var found = new List<int>();

        ForEachCandidateNumber(digits, new SetDedupeCollector(found));

        var result = found.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(result));
        return result;
    }

    // The position walk itself, shared by both strategies so the only thing they
    // differ in is the dedupe and the sort.
    private static void ForEachCandidateNumber(int[] digits, ICandidateCollector collector)
    {
        for (var hundreds = 0; hundreds < digits.Length; hundreds++)
        {
            if (digits[hundreds] == 0)
            {
                continue;
            }

            for (var tens = 0; tens < digits.Length; tens++)
            {
                if (tens == hundreds)
                {
                    continue;
                }

                EmitCandidatesForPrefix(digits, new DigitPrefix(hundreds, tens), collector);
            }
        }
    }

    private static void EmitCandidatesForPrefix(
        int[] digits, DigitPrefix prefix, ICandidateCollector collector)
    {
        for (var ones = 0; ones < digits.Length; ones++)
        {
            if (CannotBeOnesDigit(ones, prefix, digits))
            {
                continue;
            }

            var number = (digits[prefix.Hundreds] * HundredsPlace) + (digits[prefix.Tens] * TensPlace) + digits[ones];
            collector.Collect(number);
        }
    }

    // The ones position is free only when neither earlier position already claimed
    // it, and its digit is even.
    private static bool CannotBeOnesDigit(int ones, DigitPrefix prefix, int[] digits)
        => ones == prefix.Hundreds || ones == prefix.Tens || digits[ones] % OddEvenDivisor != 0;

    private readonly record struct DigitPrefix(int Hundreds, int Tens);

    // What the position walk does with each candidate it produces. Deduping is the
    // whole of it, and it is the one thing the two strategies differ in, so it is a
    // decision a caller states rather than a bare Action<int> whose only meaning was
    // "some int" - the walk itself never knows or cares which dedupe it is feeding.
    private interface ICandidateCollector
    {
        // Takes one 3-digit candidate, in the ascending position order the walk
        // visits them in. Called once per distinct position triple, so the same
        // number can arrive more than once.
        void Collect(int number);
    }

    // The baseline's dedupe: a List<int>.Contains scan of everything found so far,
    // repeated for every one of the O(n^3) candidates.
    private sealed class ListScanCollector(List<int> found) : ICandidateCollector
    {
        public void Collect(int number)
        {
            if (!found.Contains(number))
            {
                found.Add(number);
            }
        }
    }

    // This repo's own Set<int> for an O(1) expected membership check, guarding the
    // same growing result list the baseline scans linearly.
    private sealed class SetDedupeCollector(List<int> found) : ICandidateCollector
    {
        private readonly Set<int> seen = new();

        public void Collect(int number)
        {
            if (seen.TryAdd(number))
            {
                found.Add(number);
            }
        }
    }
}
