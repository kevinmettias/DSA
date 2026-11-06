using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.Solutions.FancySequence;

// LeetCode 1622. Fancy Sequence: append/addAll/multAll/getIndex over a growing
// sequence, every value reported modulo 1e9+7 and getIndex reporting -1 for an
// index that was never appended.
//
// This is a design problem - LeetCode's own shape is a stateful object with four
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// AllOneDataStructureSolution/LRUCacheSolution use for their own design problems.
// LeetCode's constructor takes no argument and the sequence grows unboundedly, so
// each strategy's factory takes none either (ARCHITECTURE.md §17.13). Beside it, an
// overload takes the number of appends a caller will make, so a caller that knows it
// sizes the segment tree once and never widens it (LeetCode's shape beside its own,
// §17.4).
//
// CreateByArrayRescan is the textbook baseline this composition has to justify
// itself against: keep the live values in a plain BCL list and rewrite every one of
// them on each addAll/multAll. That is precisely the O(n) per operation the problem
// is designed to make too slow.
//
// CreateByLazySegmentTreeAffine recognizes addAll/multAll as the one lazy-propagated
// affine transform x -> Mult*x + Add that this repo's own
// SegmentTree<Element,TUpdate,TOperation> already generalizes over via
// IRangeUpdateOperation - RangeAddSumOperation/RangeAssignMaxOperation are its two
// existing witnesses (add-then-sum, assign-then-max) and AffineOperation beside this
// file is a third - so each whole-prefix update costs O(log n) instead of O(n)
// (FallingSquaresSolution's own lazy-segment-tree-vs-rescan precedent, there with
// range-assign-max instead of an affine op).
[LeetCodeQuestion("fancy-sequence", LeetCodeAnswerOrder.AsWritten)]
internal static class FancySequenceSolution
{
    // Append plants a raw value with a single-point update whose Mult is 0 (not the
    // more obvious 1) specifically so the update can never equal AffineOperation's
    // NoUpdate = (1, 0), even when val is 0, which SegmentTree's own
    // ValidateUpdate would otherwise reject as "no pending update to apply".
    private const long PlantValueMultiplier = 0L;

    private const long IdentityMultiplier = 1L;

    private const long IdentityAddend = 0L;

    // LeetCode's constructor foresees no appends at all.
    private const int NoAppendsForeseen = 0;

    // How many times wider a full segment tree is rebuilt.
    private const int WideningFactor = 2;

    // Each strategy as LeetCode's constructor makes it, so the catalog harness can make the object its call
    // scripts run on (ARCHITECTURE 17.13).
    public static IFancySequence CreateByArrayRescan() => CreateByArrayRescan(NoAppendsForeseen);

    public static IFancySequence CreateByLazySegmentTreeAffine() => CreateByLazySegmentTreeAffine(NoAppendsForeseen);

    public static IFancySequence CreateByArrayRescan(int capacity) => new ArrayRescanFancySequence(capacity);

    public static IFancySequence CreateByLazySegmentTreeAffine(int capacity) =>
        new LazySegmentTreeAffineFancySequence(capacity);

    // LeetCode's Fancy: the four calls its scripts make, which each strategy's object
    // answers under the same names.
    internal interface IFancySequence
    {
        void Append(int val);

        void AddAll(int inc);

        void MultAll(int multiplier);

        int GetIndex(int idx);
    }

    // Deliberately written without this repo's primitives - a BCL list and a rescan
    // loop are what you would write without them.
    private sealed class ArrayRescanFancySequence(int capacity) : IFancySequence
    {
        private readonly List<long> _values = new(capacity);

        public void Append(int val) => _values.Add(val % LeetCodeModulus.Value);

        public void AddAll(int inc)
        {
            for (var i = 0; i < _values.Count; i++)
            {
                _values[i] = (_values[i] + inc) % LeetCodeModulus.Value;
            }
        }

        public void MultAll(int multiplier)
        {
            for (var i = 0; i < _values.Count; i++)
            {
                _values[i] = _values[i] * multiplier % LeetCodeModulus.Value;
            }
        }

        public int GetIndex(int idx) =>
            idx >= _values.Count ? LeetCodeAnswer.None : ValueAt(idx);

        private int ValueAt(int idx) => (int)_values[idx];
    }

    // The segment tree is as wide as the capacity it is given. An append past that
    // width rebuilds it twice as wide, so a sequence of unknown length - LeetCode's -
    // still pays O(log n) amortized per append, as a List's doubling keeps its own Add
    // O(1) amortized.
    private sealed class LazySegmentTreeAffineFancySequence(int capacity) : IFancySequence
    {
        private SegmentTree<long, (long Mult, long Add), AffineOperation> _tree = new(new long[capacity]);

        private int _size;

        public void Append(int val)
        {
            if (_size == _tree.Count)
            {
                _tree = Widened();
            }

            _tree.UpdateRange(_size, _size, (PlantValueMultiplier, val));
            _size++;
        }

        // A tree twice as wide whose leaves are the values this one reads now, every
        // pending update already applied to them, so nothing is left pending.
        private SegmentTree<long, (long Mult, long Add), AffineOperation> Widened()
        {
            var width = Math.Max(1, _tree.Count * WideningFactor);
            var values = new long[width];

            for (var i = 0; i < _size; i++)
            {
                values[i] = _tree.Query(i, i);
            }

            return new(values);
        }

        // An addAll of 0 and a multAll of 1 are both the affine identity, which is
        // AffineOperation.NoUpdate - reserved by SegmentTree.UpdateRange to mean "nothing
        // pending" - so they are skipped rather than forwarded. Skipping is also the
        // correct answer: neither changes a single value.
        public void AddAll(int inc)
        {
            if (_size > 0 && inc != IdentityAddend)
            {
                _tree.UpdateRange(0, _size - 1, (IdentityMultiplier, inc));
            }
        }

        public void MultAll(int multiplier)
        {
            if (_size > 0 && multiplier != IdentityMultiplier)
            {
                _tree.UpdateRange(0, _size - 1, (multiplier, IdentityAddend));
            }
        }

        public int GetIndex(int idx) =>
            idx >= _size ? LeetCodeAnswer.None : (int)_tree.Query(idx, idx);
    }
}
