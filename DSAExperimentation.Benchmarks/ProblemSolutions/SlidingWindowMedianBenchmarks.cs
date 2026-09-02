using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sliding Window Median (LC 480): the O(n*k log k) baseline most people reach for
// first - copy each k-sized window and Array.Sort it from scratch - vs. this repo's
// own two Heap<T,TOrder> instances (MaxHeapOrder<int> lower half, MinHeapOrder<int>
// upper half) plus HashMap<int,int> for lazy deletion, the O(n log k) two-heap
// approach SlidingWindowMedianTests.cs's SlidingWindowMedianOperations already uses,
// extending FindMedianFromDataStreamTests' MedianFinderOperations with a
// window-eviction Erase. WindowSize is kept well below Length so both strategies do
// real repeated work across many windows, not one giant one.
[MemoryDiagnoser]
public class SlidingWindowMedianBenchmarks
{
    private const int WindowSize = 500;

    private const int RandomValueUpperBound = 10_000;

    // Index into the sorted window used by SortEachWindow's approximate median.
    private const int MedianIndexDivisor = 2;

    [Params(2_000, 8_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, RandomValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public double SortEachWindow()
    {
        var lastMedian = 0.0;
        var window = new int[WindowSize];

        for (var start = 0; start <= _values.Length - WindowSize; start++)
        {
            Array.Copy(_values, start, window, 0, WindowSize);
            Array.Sort(window);
            lastMedian = window[WindowSize / MedianIndexDivisor];
        }

        return lastMedian;
    }

    [Benchmark]
    public double TwoHeapsLazyDeletion()
    {
        var window = new SlidingWindowMedianOperations();
        var lastMedian = 0.0;

        for (var i = 0; i < _values.Length; i++)
        {
            window.Insert(_values[i]);

            if (i >= WindowSize - 1)
            {
                lastMedian = window.Median();
                window.Erase(_values[i - WindowSize + 1]);
            }
        }

        return lastMedian;
    }

    private sealed class SlidingWindowMedianOperations
    {
        // Divisor for averaging the two heap tops when the window has an even count.
        private const double TwoValueAverageDivisor = 2.0;

        private readonly Heap<int, MaxHeapOrder<int>> _lower = new();
        private readonly Heap<int, MinHeapOrder<int>> _upper = new();
        private readonly HashMap<int, int> _delayed = new();
        private int _lowerSize;
        private int _upperSize;

        public void Insert(int num)
        {
            if (_lowerSize == 0 || num <= PeekLower())
            {
                _lower.Push(num);
                _lowerSize++;
            }
            else
            {
                _upper.Push(num);
                _upperSize++;
            }

            Rebalance();
        }

        public void Erase(int num)
        {
            MarkDelayed(num);

            if (num <= PeekLower())
            {
                _lowerSize--;

                if (num == PeekLower())
                {
                    PruneLower();
                }
            }
            else
            {
                _upperSize--;

                if (num == PeekUpper())
                {
                    PruneUpper();
                }
            }

            Rebalance();
        }

        public double Median()
            => _lowerSize == _upperSize
                ? (PeekLower() + PeekUpper()) / TwoValueAverageDivisor
                : PeekLower();

        private void Rebalance()
        {
            if (_lowerSize > _upperSize + 1)
            {
                var moved = PeekLower();
                _lower.TryPop(out _);
                _lowerSize--;
                _upper.Push(moved);
                _upperSize++;
                PruneLower();
            }
            else if (_upperSize > _lowerSize)
            {
                var moved = PeekUpper();
                _upper.TryPop(out _);
                _upperSize--;
                _lower.Push(moved);
                _lowerSize++;
                PruneUpper();
            }
        }

        private void PruneLower()
        {
            while (_lower.TryPeek(out var top) && IsDelayed(top))
            {
                ClearOneDelayed(top);
                _lower.TryPop(out _);
            }
        }

        private void PruneUpper()
        {
            while (_upper.TryPeek(out var top) && IsDelayed(top))
            {
                ClearOneDelayed(top);
                _upper.TryPop(out _);
            }
        }

        private bool IsDelayed(int value) => _delayed.TryGetValue(value, out var count) && count > 0;

        private void MarkDelayed(int value)
        {
            _delayed.TryGetValue(value, out var count);
            _delayed.Set(value, count + 1);
        }

        private void ClearOneDelayed(int value)
        {
            _delayed.TryGetValue(value, out var count);

            if (count <= 1)
            {
                _delayed.TryRemove(value);
            }
            else
            {
                _delayed.Set(value, count - 1);
            }
        }

        private int PeekLower()
        {
            _lower.TryPeek(out var value);
            return value;
        }

        private int PeekUpper()
        {
            _upper.TryPeek(out var value);
            return value;
        }
    }
}
