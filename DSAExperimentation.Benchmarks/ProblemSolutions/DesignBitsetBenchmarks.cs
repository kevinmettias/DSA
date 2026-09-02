using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Bitset (LC 2166): a naive bool[]-backed bitset whose flip() physically
// toggles every stored bit (O(size) - the natural first-pass implementation) vs.
// this repo's DynamicArray<int>-backed bitset (DesignBitsetTests) whose flip() only
// toggles a lazy "flipped" interpretation flag and mirrors the running ones-count
// (size - count), so the backing array is never touched - O(1). Each [Benchmark]
// fixes/unfixes a scattered subset of indices, then runs Size flip() calls (each
// followed by a count() read, so the flag's effect is actually observed) - the
// operation flip() exists specifically to make O(1) instead of O(size).
[MemoryDiagnoser]
public class DesignBitsetBenchmarks
{
    private const int FixEveryNth = 3;
    private const int UnfixEveryNth = 5;

    [Params(500, 20_000)]
    public int Size;

    [Benchmark(Baseline = true)]
    public long EagerArrayFlip()
    {
        var bitset = new EagerBitset(Size);
        return RunWorkload(bitset);
    }

    [Benchmark]
    public long LazyFlagDynamicArray()
    {
        var bitset = new LazyBitset(Size);
        return RunWorkload(bitset);
    }

    private long RunWorkload(IBenchmarkBitset bitset)
    {
        for (var i = 0; i < Size; i++)
        {
            if (i % FixEveryNth == 0)
            {
                bitset.Fix(i);
            }

            if (i % UnfixEveryNth == 0)
            {
                bitset.Unfix(i);
            }
        }

        long total = 0;

        for (var i = 0; i < Size; i++)
        {
            bitset.Flip();
            total += bitset.Count();
        }

        return total;
    }

    private interface IBenchmarkBitset
    {
        void Fix(int idx);

        void Unfix(int idx);

        void Flip();

        int Count();
    }

    // The naive baseline: flip() walks and physically negates every element.
    private sealed class EagerBitset(int size) : IBenchmarkBitset
    {
        private readonly bool[] _bits = new bool[size];
        private int _onesCount;

        public void Fix(int idx)
        {
            if (!_bits[idx])
            {
                _bits[idx] = true;
                _onesCount++;
            }
        }

        public void Unfix(int idx)
        {
            if (_bits[idx])
            {
                _bits[idx] = false;
                _onesCount--;
            }
        }

        public void Flip()
        {
            for (var i = 0; i < _bits.Length; i++)
            {
                _bits[i] = !_bits[i];
            }

            _onesCount = _bits.Length - _onesCount;
        }

        public int Count() => _onesCount;
    }

    // This repo's DynamicArray<int> as the fixed-size raw-bit store, with an
    // unapplied "flipped" flag standing in for a real per-element toggle.
    private sealed class LazyBitset : IBenchmarkBitset
    {
        private readonly DynamicArray<int> _bits = new();
        private readonly int _size;
        private bool _flipped;
        private int _onesCount;

        public LazyBitset(int size)
        {
            _size = size;

            for (var i = 0; i < size; i++)
            {
                _bits.Add(0);
            }
        }

        public void Fix(int idx) => SetVisible(idx, targetVisible: 1);

        public void Unfix(int idx) => SetVisible(idx, targetVisible: 0);

        public void Flip()
        {
            _flipped = !_flipped;
            _onesCount = _size - _onesCount;
        }

        public int Count() => _onesCount;

        private void SetVisible(int idx, int targetVisible)
        {
            var visible = _bits.Get(idx) ^ (_flipped ? 1 : 0);

            if (visible == targetVisible)
            {
                return;
            }

            var targetRaw = _flipped ? 1 - targetVisible : targetVisible;
            _bits.Set(idx, targetRaw);
            _onesCount += targetVisible == 1 ? 1 : -1;
        }
    }
}
