using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Statistics from a Large Sample (LC 1093): expanding the 256-bucket count array into
// the full sorted sample and indexing straight into it for the median vs. this repo's
// own BinarySearch.LowerBound over a cumulative-sum ArraySequence<long> built directly
// from the buckets. AverageCountPerValue scales the total sample size while the bucket
// range stays fixed at [0, 255] - the shape LeetCode itself fixes - so ExpandAndIndex's
// O(total) allocation grows while CumulativeSumBinarySearch's stays O(256) regardless.
[MemoryDiagnoser]
public class StatisticsFromALargeSampleBenchmarks
{
    private const int ValueRange = 256;

    [Params(100, 5_000)]
    public int AverageCountPerValue;

    private long[] _count = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _count = new long[ValueRange];

        for (var i = 0; i < ValueRange; i++)
        {
            _count[i] = random.Next(1, (AverageCountPerValue * 2) + 1);
        }
    }

    [Benchmark(Baseline = true)]
    public double ExpandAndIndex() => MedianByExpansion(_count);

    [Benchmark]
    public double CumulativeSumBinarySearch() => MedianByBinarySearch(_count);

    private static double MedianByExpansion(long[] count)
    {
        long total = 0;
        foreach (var c in count)
        {
            total += c;
        }

        var sample = new int[total];
        var index = 0;

        for (var value = 0; value < count.Length; value++)
        {
            for (long occurrence = 0; occurrence < count[value]; occurrence++)
            {
                sample[index++] = value;
            }
        }

        return total % 2 == 1
            ? sample[total / 2]
            : (sample[(total / 2) - 1] + sample[total / 2]) / 2.0;
    }

    private static double MedianByBinarySearch(long[] count)
    {
        var cumulative = new long[count.Length];
        long running = 0;

        for (var i = 0; i < count.Length; i++)
        {
            running += count[i];
            cumulative[i] = running;
        }

        var total = running;
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
