using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignBitset;

// LeetCode 2166. Design Bitset: a fixed-size raw-bit array (this repo's own
// DynamicArray<int>, preallocated to `size` via Add - the same "compose an existing
// Representation behind a thin design-problem wrapper" shape DesignCircularDequeTests
// wraps around Deque<int>) plus a lazily-applied "flipped" interpretation flag and a
// running ones-count. flip() therefore never touches the backing array - it just
// toggles the flag and mirrors the count (size - count) - which is the whole reason
// this problem exists: an eager flip() that rewrites every element is O(size), while
// this lazy flag keeps flip()/count()/all()/one() all O(1). fix/unfix compare the
// bit's CURRENT visible value (raw XOR flipped) against the desired one and only
// touch the array - and adjust the running count - when a change is actually needed.
public sealed partial class DesignBitsetTests
{
    // LeetCode's own worked example: "00000" -> fix(3) -> fix(4) -> flip() -> all()
    // is False -> unfix(0) -> flip(), ending at "10011".
    [Fact]
    public void FixFlipUnfixFlip_LeetCodeExample_MatchesExpectedStateAtEachStep()
    {
        var bitset = new Bitset(5);

        bitset.Fix(3);
        bitset.Fix(4);
        Assert.Equal("00011", bitset.ToBitString());

        bitset.Flip();
        Assert.Equal("11100", bitset.ToBitString());
        Assert.False(bitset.All());

        bitset.Unfix(0);
        Assert.Equal("01100", bitset.ToBitString());

        bitset.Flip();
        Assert.Equal("10011", bitset.ToBitString());
        Assert.Equal(3, bitset.Count());
        Assert.True(bitset.One());
    }

    [Fact]
    public void All_EveryBitFixed_ReturnsTrue()
    {
        var bitset = new Bitset(3);

        bitset.Fix(0);
        bitset.Fix(1);
        bitset.Fix(2);

        Assert.True(bitset.All());
        Assert.Equal(3, bitset.Count());
    }

    [Fact]
    public void One_NoBitsEverFixed_ReturnsFalse()
    {
        var bitset = new Bitset(4);

        Assert.False(bitset.One());
        Assert.Equal(0, bitset.Count());
        Assert.Equal("0000", bitset.ToBitString());
    }

    [Fact]
    public void FlipTwice_ReturnsToOriginalState()
    {
        var bitset = new Bitset(4);
        bitset.Fix(1);

        bitset.Flip();
        bitset.Flip();

        Assert.Equal("0100", bitset.ToBitString());
        Assert.Equal(1, bitset.Count());
    }

    private sealed class Bitset
    {
        private readonly DynamicArray<int> _bits = new();
        private readonly int _size;
        private bool _flipped;
        private int _onesCount;

        public Bitset(int size)
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
                chars[i] = Visible(i) == 1 ? '1' : '0';
            }

            return new string(chars);
        }

        private void SetVisible(int idx, int targetVisible)
        {
            if (Visible(idx) == targetVisible)
            {
                return;
            }

            var targetRaw = _flipped ? 1 - targetVisible : targetVisible;
            _bits.Set(idx, targetRaw);
            _onesCount += targetVisible == 1 ? 1 : -1;
        }

        private int Visible(int idx) => _bits.Get(idx) ^ (_flipped ? 1 : 0);
    }
}
