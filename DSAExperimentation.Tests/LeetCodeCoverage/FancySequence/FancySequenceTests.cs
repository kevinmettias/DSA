using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FancySequence;

// LeetCode 1622. Fancy Sequence: append/addAll/multAll/getIndex is exactly the
// lazy-propagated affine transform x -> mult*x + add that this repo's own
// LazySegmentTree<Element,TUpdate,TOperation> already generalizes over via
// IRangeUpdateOperation<Element,TUpdate> - RangeAddSumOperation/RangeAssignMax
// Operation are its two existing witnesses (add-then-sum, assign-then-max);
// AffineOperation below is a third, composing the same generic engine the way
// NumberOfLongestIncreasingSubsequenceTests' LisAggregate composes SegmentTree's
// own ICombineOperation. append plants a new leaf's raw value with a single-point
// UpdateRange(size, size, (Mult:0, Add:val)) - Mult:0 (not the more obvious Mult:1)
// specifically so the update can never collide with NoUpdate=(1,0), even when
// val=0, which LazySegmentTree's own ValidateUpdate would otherwise reject as "no
// pending update to apply." addAll/multAll range-update the live prefix
// [0, size); getIndex reads a single leaf back, mod 1e9+7 as LeetCode requires.
public sealed partial class FancySequenceTests
{
    [Fact]
    public void AppendAddAllMultAllGetIndex_LeetCodeExampleSequence_MatchesExpectedValues()
    {
        var fancy = new FancySequence(4);

        fancy.Append(2); // [2]
        fancy.AddAll(3); // [5]
        fancy.Append(7); // [5, 7]
        fancy.MultAll(2); // [10, 14]
        Assert.Equal(10, fancy.GetIndex(0));

        fancy.AddAll(4); // [14, 18]
        fancy.MultAll(2); // [28, 36]
        Assert.Equal(28, fancy.GetIndex(0));
        Assert.Equal(36, fancy.GetIndex(1));
    }

    [Fact]
    public void GetIndex_IndexNeverAppended_ReturnsNegativeOne()
    {
        var fancy = new FancySequence(2);
        fancy.Append(5);

        Assert.Equal(-1, fancy.GetIndex(1));
    }

    [Fact]
    public void Append_ValueOfZero_DoesNotCollideWithTheNoUpdateSentinel()
    {
        var fancy = new FancySequence(1);

        fancy.Append(0);

        Assert.Equal(0, fancy.GetIndex(0));
    }

    private const long Modulo = 1_000_000_007;

    private sealed class FancySequence(int capacity)
    {
        private readonly LazySegmentTree<long, (long Mult, long Add), AffineOperation> _tree = new(new long[capacity]);
        private int _size;

        public void Append(int val)
        {
            _tree.UpdateRange(_size, _size, (0L, val));
            _size++;
        }

        public void AddAll(int inc)
        {
            if (_size > 0 && inc != 0)
            {
                _tree.UpdateRange(0, _size - 1, (1L, inc));
            }
        }

        public void MultAll(int m)
        {
            if (_size > 0 && m != 1)
            {
                _tree.UpdateRange(0, _size - 1, (m, 0L));
            }
        }

        public int GetIndex(int idx) => idx >= _size ? -1 : (int)_tree.Query(idx, idx);
    }

    // Identity/Combine are never actually read back by a point-only Query
    // (LazySegmentTree.Query's full-cover branch only returns a node's own
    // _values for a query that spans the node's whole range, which for a
    // single-leaf query only ever happens at the leaf itself), so any associative
    // pairing satisfies the algebra - plain mod-sum mirrors RangeAddSumOperation's
    // own choice. NoUpdate=(1,0) is the affine identity x -> x; ComposeUpdate folds
    // a newly arriving affine transform on top of one already pending exactly the
    // way function composition does: outer(inner(x)) = outer.Mult*inner.Mult*x +
    // (outer.Mult*inner.Add + outer.Add).
    private readonly struct AffineOperation : IRangeUpdateOperation<long, (long Mult, long Add)>
    {
        public static long Identity => 0L;

        public static (long Mult, long Add) NoUpdate => (1L, 0L);

        public static long Combine(long left, long right) => (left + right) % Modulo;

        public static (long Mult, long Add) ComposeUpdate((long Mult, long Add) outer, (long Mult, long Add) inner)
            => (outer.Mult * inner.Mult % Modulo, (outer.Mult * inner.Add + outer.Add) % Modulo);

        public static long ApplyUpdate(long aggregate, (long Mult, long Add) update, int rangeLength)
            => (update.Mult * aggregate + update.Add) % Modulo;
    }
}
