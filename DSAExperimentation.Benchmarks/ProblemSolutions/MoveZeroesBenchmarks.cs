using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Move Zeroes (LC 283): the naive "rescan for the next nonzero from i every time"
// approach - O(n^2) worst case - vs. the O(n) ArrayIndexedSequence<T> two-pointer
// swap SortColors' Dutch-flag partition already proves out for this repo, specialized
// to a single zero/nonzero split. _values is deliberately front-loaded with zeroes so
// the naive scan's "how far to the next nonzero" grows every iteration instead of
// finding one immediately, forcing its real worst-case cost.
[MemoryDiagnoser]
public class MoveZeroesBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i < Length / 2 ? 0 : i + 1).ToArray();

    [Benchmark(Baseline = true)]
    public int LinearScanForNextNonZero()
    {
        var nums = _values.ToArray();

        for (var i = 0; i < nums.Length; i++)
        {
            if (nums[i] != 0)
            {
                continue;
            }

            var j = i + 1;
            while (j < nums.Length && nums[j] == 0)
            {
                j++;
            }

            if (j < nums.Length)
            {
                (nums[i], nums[j]) = (nums[j], nums[i]);
            }
        }

        return nums[0];
    }

    [Benchmark]
    public int ArrayIndexedTwoPointer()
    {
        var nums = _values.ToArray();
        var seq = new ArrayIndexedSequence<int>(nums);
        var insertPos = 0;

        for (var i = 0; i < seq.Length; i++)
        {
            if (seq.Get(i) != 0)
            {
                Swap(seq, insertPos++, i);
            }
        }

        return nums[0];
    }

    private static void Swap(ArrayIndexedSequence<int> seq, int first, int second)
    {
        var temp = seq.Get(first);
        seq.Set(first, seq.Get(second));
        seq.Set(second, temp);
    }
}
