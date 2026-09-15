using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.DesignBitset;

// LeetCode 2166. Design Bitset: a fixed-size array of bits supporting fix, unfix,
// flip, all, one, count and a string rendering.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and seven operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md section 17.3) takes the form of two full classes
// implementing the shared IBitset surface below, the same shape
// DesignCircularDequeSolution uses for its own instance-API problem (LC 641).
//
// The whole point of the problem is flip(): the natural first pass rewrites every
// stored bit, which is O(size), while carrying the flip as an unapplied
// interpretation flag and mirroring the running ones-count (size - count) keeps
// flip/count/all/one all O(1).
internal static class DesignBitsetSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it. ToBitString is LeetCode's toString(), renamed so it does not
    // collide with object.ToString.
    internal interface IBitset
    {
        void Fix(int idx);

        void Unfix(int idx);

        void Flip();

        bool All();

        bool One();

        int Count();

        string ToBitString();
    }

    // The textbook baseline this composition has to justify itself against: a plain
    // BCL bool[] whose Flip walks the whole array and physically negates every
    // element. Deliberately written without this repo's primitives - it is the arm
    // the lazy strategy below has to beat.
    internal sealed class BitsetByEagerFlip(int size) : IBitset
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

        public bool All() => _onesCount == _bits.Length;

        public bool One() => _onesCount > 0;

        public int Count() => _onesCount;

        public string ToBitString()
        {
            var chars = new char[_bits.Length];

            for (var i = 0; i < _bits.Length; i++)
            {
                var bitIsSet = _bits[i];
                chars[i] = bitIsSet ? '1' : '0';
            }

            return new string(chars);
        }
    }

    // The composed answer: this repo's own DynamicArray<int> as the fixed-size raw
    // bit store - preallocated to `size` via Add, the same "compose an existing
    // Representation behind a thin design-problem wrapper" move
    // DesignCircularDequeSolution makes over Deque<int> - plus a lazily applied
    // "flipped" interpretation flag and a running ones-count. Flip therefore never
    // touches the backing store: it toggles the flag and mirrors the count. Fix and
    // Unfix compare the bit's CURRENT visible value (raw XOR flipped) against the
    // desired one and only write - and only adjust the count - when a change is
    // actually needed.
    internal sealed class BitsetByLazyFlag : IBitset
    {
        private readonly DynamicArray<int> _bits = new();
        private readonly int _size;
        private bool _flipped;
        private int _onesCount;

        public BitsetByLazyFlag(int size)
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

        public bool All() => _onesCount == _size;

        public bool One() => _onesCount > 0;

        public int Count() => _onesCount;

        public string ToBitString()
        {
            var chars = new char[_size];

            for (var i = 0; i < _size; i++)
            {
                var bitIsVisible = Visible(i) == 1;
                chars[i] = bitIsVisible ? '1' : '0';
            }

            return new string(chars);
        }

        private void SetVisible(int idx, int targetVisible)
        {
            if (Visible(idx) == targetVisible)
            {
                return;
            }

            var targetRaw = _flipped ? ComplementBit(targetVisible) : targetVisible;
            _bits.Set(idx, targetRaw);
            _onesCount += targetVisible == 1 ? 1 : -1;
        }

        private int Visible(int idx) => _bits.Get(idx) ^ (_flipped ? 1 : 0);

        // The raw bit stored when the buffer is read through the flipped flag:
        // the complement of the value the caller wants to see.
        private static int ComplementBit(int visible) => 1 - visible;
    }
}
