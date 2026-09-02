using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StatisticsFromALargeSample;

// LeetCode 1093. Statistics from a Large Sample: min/max/mean/mode fall out of a
// single pass over the 256-bucket count array, but median is a positional query
// ("the element(s) at sorted position total/2") over that array's running cumulative
// sum - exactly the shape this repo's own BinarySearch.LowerBound answers ("first
// index whose value is >= target") over an ArraySequence<long>, without ever
// materializing the up-to-1e9-element sample itself.
public sealed partial class StatisticsFromALargeSampleTests
{
    [Fact]
    public void ComputeStatistics_OddSampleSize_ReturnsSinglePositionMedian()
    {
        // sample [1,2,2,2,3]: count[1]=1, count[2]=3, count[3]=1.
        var count = new long[256];
        count[1] = 1;
        count[2] = 3;
        count[3] = 1;

        var stats = ComputeStatistics(count);

        Assert.Equal([1.0, 3.0, 2.0, 2.0, 2.0], stats);
    }

    [Fact]
    public void ComputeStatistics_EvenSampleSize_AveragesTheTwoMiddlePositions()
    {
        // sample [1,1,2,3]: count[1]=2, count[2]=1, count[3]=1.
        var count = new long[256];
        count[1] = 2;
        count[2] = 1;
        count[3] = 1;

        var stats = ComputeStatistics(count);

        Assert.Equal([1.0, 3.0, 1.75, 1.5, 1.0], stats);
    }

    private static double[] ComputeStatistics(long[] count)
    {
        var accumulator = new SampleAccumulator();

        for (var value = 0; value < count.Length; value++)
        {
            accumulator.Accumulate(value, count[value]);
        }

        var mean = (double)accumulator.WeightedSum / accumulator.Total;
        var median = ComputeMedian(count, accumulator.Total);

        return [accumulator.Min, accumulator.Max, mean, median, accumulator.ModeValue];
    }

    private struct SampleAccumulator
    {
        public int Min = -1;
        public int Max = -1;
        public long Total;
        public long WeightedSum;
        public int ModeValue;
        public long ModeCount;

        public SampleAccumulator()
        {
        }

        public void Accumulate(int value, long occurrences)
        {
            if (occurrences == 0)
            {
                return;
            }

            if (Min == -1)
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

    private static double ComputeMedian(long[] count, long total)
    {
        var cumulative = new long[count.Length];
        long running = 0;

        for (var i = 0; i < count.Length; i++)
        {
            running += count[i];
            cumulative[i] = running;
        }

        var sequence = new ArraySequence<long>(cumulative);

        if (total % 2 == 1)
        {
            return BinarySearch.LowerBound(sequence, total / 2 + 1);
        }

        var lowerMiddle = BinarySearch.LowerBound(sequence, total / 2);
        var upperMiddle = BinarySearch.LowerBound(sequence, total / 2 + 1);
        return (lowerMiddle + upperMiddle) / 2.0;
    }
}
