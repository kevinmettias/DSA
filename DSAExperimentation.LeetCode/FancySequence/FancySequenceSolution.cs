using DSAExperimentation.DataStructures.LazySegmentTree;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FancySequence;

// LeetCode 1622. Fancy Sequence: append/addAll/multAll/getIndex over a growing
// sequence, every value reported modulo 1e9+7 and getIndex reporting -1 for an
// index that was never appended.
//
// This is a design problem - LeetCode's own shape is a stateful object with four
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// AllOneDataStructureSolution/LRUCacheSolution use for their own design problems.
// Both factories take the number of appends the caller will make, because the
// composed strategy's LazySegmentTree is a fixed-width structure; LeetCode's own
// constructor takes no argument and grows unboundedly.
//
// CreateByArrayRescan is the textbook baseline this composition has to justify
// itself against: keep the live values in a plain BCL list and rewrite every one of
// them on each addAll/multAll. That is precisely the O(n) per operation the problem
// is designed to make too slow.
//
// CreateByLazySegmentTreeAffine recognizes addAll/multAll as the one lazy-propagated
// affine transform x -> Mult*x + Add that this repo's own
// LazySegmentTree<Element,TUpdate,TOperation> already generalizes over via
// IRangeUpdateOperation - RangeAddSumOperation/RangeAssignMaxOperation are its two
// existing witnesses (add-then-sum, assign-then-max) and AffineOperation beside this
// file is a third - so each whole-prefix update costs O(log n) instead of O(n)
// (FallingSquaresSolution's own LazySegmentTree-vs-rescan precedent, there with
// range-assign-max instead of an affine op).
internal static class FancySequenceSolution
{
    // Append plants a raw value with a single-point update whose Mult is 0 (not the
    // more obvious 1) specifically so the update can never equal AffineOperation's
    // NoUpdate = (1, 0), even when val is 0, which LazySegmentTree's own
    // ValidateUpdate would otherwise reject as "no pending update to apply".
    private const long PlantValueMultiplier = 0L;

    private const long IdentityMultiplier = 1L;

    private const long IdentityAddend = 0L;

    public static IFancySequence CreateByArrayRescan(int capacity) => new ArrayRescanFancySequence(capacity);

    public static IFancySequence CreateByLazySegmentTreeAffine(int capacity) =>
        new LazySegmentTreeAffineFancySequence(capacity);

    internal interface IFancySequence
    {
        void Append(int val);

        void AddAll(int inc);

        void MultAll(int m);

        int GetIndex(int idx);
    }

    // Deliberately written without this repo's primitives - a BCL list and a rescan
    // loop are what you would write without them.
    private sealed class ArrayRescanFancySequence(int capacity) : IFancySequence
    {
        private readonly List<long> _values = new(capacity);

        public void Append(int val) => _values.Add(val % ModularArithmetic.Modulo);

        public void AddAll(int inc)
        {
            for (var i = 0; i < _values.Count; i++)
            {
                _values[i] = (_values[i] + inc) % ModularArithmetic.Modulo;
            }
        }

        public void MultAll(int m)
        {
            for (var i = 0; i < _values.Count; i++)
            {
                _values[i] = _values[i] * m % ModularArithmetic.Modulo;
            }
        }

        public int GetIndex(int idx) =>
            idx >= _values.Count ? LeetCodeAnswer.None : (int)_values[idx];
    }

    private sealed class LazySegmentTreeAffineFancySequence(int capacity) : IFancySequence
    {
        private readonly LazySegmentTree<long, (long Mult, long Add), AffineOperation> _tree = new(new long[capacity]);

        private int _size;

        public void Append(int val)
        {
            _tree.UpdateRange(_size, _size, (PlantValueMultiplier, val));
            _size++;
        }

        // An addAll of 0 and a multAll of 1 are both the affine identity, which is
        // AffineOperation.NoUpdate - reserved by LazySegmentTree to mean "nothing
        // pending" - so they are skipped rather than forwarded. Skipping is also the
        // correct answer: neither changes a single value.
        public void AddAll(int inc)
        {
            if (_size > 0 && inc != IdentityAddend)
            {
                _tree.UpdateRange(0, _size - 1, (IdentityMultiplier, inc));
            }
        }

        public void MultAll(int m)
        {
            if (_size > 0 && m != IdentityMultiplier)
            {
                _tree.UpdateRange(0, _size - 1, (m, IdentityAddend));
            }
        }

        public int GetIndex(int idx) =>
            idx >= _size ? LeetCodeAnswer.None : (int)_tree.Query(idx, idx);
    }
}
