using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.StatisticsFromALargeSample;

// LeetCode 1093. Statistics from a Large Sample: given the occurrence count of
// every value in [0, 255], report [minimum, maximum, mean, median, mode] of the
// sample those counts describe - a sample that may hold up to 1e9 elements.
//
// Minimum, maximum, mean and mode all fall out of one pass over the buckets. The
// median is the only positional query: "the value at sorted position total/2".
// The two strategies differ in how they answer it - expanding the buckets into the
// whole sorted sample and indexing straight into it, or one BinarySearch.LowerBound
// over the buckets' running cumulative sum, which is "first index whose value is
// >= target" and therefore never materializes the sample at all.
internal static class StatisticsFromALargeSampleSolution
{
    // total % MedianParityDivisor distinguishes an odd total (one middle element)
    // from an even total (two middle elements to average).
    private const int MedianParityDivisor = 2;

    // total / MedianIndexDivisor locates the middle index/indices of the sorted
    // sample.
    private const int MedianIndexDivisor = 2;

    // Divisor used to average the two middle values of an even-sized sample.
    private const double MedianPairAverageDivisor = 2.0;

    // Sentinel for "no non-empty bucket has been seen yet"; every sample value is
    // non-negative, so no real value collides with it.
    private const int NoValueSeen = -1;

    // The textbook answer: rebuild the sorted sample the counts describe and read
    // its statistics off directly. Deliberately BCL-only - it is the arm the
    // composed strategy below has to justify itself against - and it pays O(total)
    // time and allocation for a median the buckets already determine.
    public static double[] ComputeStatisticsBySampleExpansion(long[] count)
    {
        var accumulator = new SampleAccumulator();
        var sample = ExpandSample(count, accumulator);
        var total = accumulator.Total;
        var weightedSum = accumulator.WeightedSum;

        var totalIsOdd = total % MedianParityDivisor == 1;
        var median = totalIsOdd
            ? MiddleElement(sample, total)
            : AveragedMiddlePair(sample, total);

        return [sample[0], sample[total - 1], (double)weightedSum / total, median, accumulator.ModeValue];
    }

    // One pass over the buckets rebuilds the sorted sample they describe, folding each
    // bucket's order-independent statistics into the accumulator as it goes.
    private static int[] ExpandSample(long[] count, SampleAccumulator accumulator)
    {
        var total = TotalCount(count);
        var sample = new int[total];
        long index = 0;

        for (var value = 0; value < count.Length; value++)
        {
            accumulator.Accumulate(value, count[value]);

            for (long occurrence = 0; occurrence < count[value]; occurrence++)
            {
                sample[index++] = value;
            }
        }

        return sample;
    }

    // The sample's size: how many elements the buckets describe in total.
    private static long TotalCount(long[] count)
    {
        long total = 0;

        foreach (var occurrences in count)
        {
            total += occurrences;
        }

        return total;
    }

    // An odd total has one middle element, and it is the median bare.
    private static int MiddleElement(int[] sample, long total)
        => sample[total / MedianIndexDivisor];

    // An even total straddles two middle indices instead, and the median is their
    // average.
    private static double AveragedMiddlePair(int[] sample, long total)
    {
        var lowerMiddle = sample[(total / MedianIndexDivisor) - 1];
        var upperMiddle = sample[total / MedianIndexDivisor];

        return (lowerMiddle + upperMiddle) / MedianPairAverageDivisor;
    }

    // This repo's own answer: one pass builds the running cumulative sum of the
    // buckets, which is sorted by construction, so the positional median query is
    // exactly BinarySearch.LowerBound over an ArraySequence<long> of it. The index
    // LowerBound lands on IS the sample value, because bucket i holds value i, so
    // the sample is never materialized and the work stays O(count.Length)
    // regardless of how large the sample is.
    public static double[] ComputeStatisticsByCumulativeBinarySearch(long[] count)
    {
        var accumulator = new SampleAccumulator();
        var cumulative = new long[count.Length];
        long running = 0;

        for (var value = 0; value < count.Length; value++)
        {
            accumulator.Accumulate(value, count[value]);
            running += count[value];
            cumulative[value] = running;
        }

        var mean = (double)accumulator.WeightedSum / accumulator.Total;
        var median = MedianFromCumulative(cumulative, accumulator.Total);

        return [accumulator.Min, accumulator.Max, mean, median, accumulator.ModeValue];
    }

    // The four order-independent statistics, folded over the buckets in ascending
    // value order: the first non-empty bucket is the minimum, the last is the
    // maximum, and the largest bucket is the mode (LeetCode guarantees it is
    // unique, so first-wins on a tie is never observable).
    private sealed class SampleAccumulator
    {
        public int Min { get; set; } = NoValueSeen;
        public int Max { get; set; } = NoValueSeen;
        public long Total { get; set; }
        public long WeightedSum { get; set; }
        public int ModeValue { get; set; }
        public long ModeCount { get; set; }

        public SampleAccumulator()
        {
        }

        public void Accumulate(int value, long occurrences)
        {
            if (occurrences == 0)
            {
                return;
            }

            if (Min == NoValueSeen)
            {
                Min = value;
            }

            Max = value;
            Total += occurrences;
            WeightedSum += (long)value * occurrences;

            if (occurrences > ModeCount)
            {
                ModeCount = occurrences;
                ModeValue = value;
            }
        }
    }

    // cumulative[i] is how many sample elements are <= i, so "the value at sorted
    // position p" (1-based) is the first index whose cumulative count reaches p -
    // LowerBound's definition exactly.
    private static double MedianFromCumulative(long[] cumulative, long total)
    {
        var sequence = new ArraySequence<long>(cumulative);

        if (total % MedianParityDivisor == 1)
        {
            return BinarySearch.LowerBound(sequence, (total / MedianIndexDivisor) + 1);
        }

        var lowerMiddle = BinarySearch.LowerBound(sequence, total / MedianIndexDivisor);
        var upperMiddle = BinarySearch.LowerBound(sequence, (total / MedianIndexDivisor) + 1);

        return (lowerMiddle + upperMiddle) / MedianPairAverageDivisor;
    }
}
