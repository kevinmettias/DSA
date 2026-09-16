using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MaximizeActiveSectionWithTradeII;

// The static, per-string index MaxActiveAfterTradeByRangeMaxIndex composes
// against: the total '1' count in text (constant across every query - text and
// the query list never change, only which spots a trade touches), every maximal
// run of '0's in text, a per-position pointer to "the most recent zero-run
// starting at or before this position", and a
// SegmentTree<int, MaxOperation<int>> over the pairwise sum of every two
// CONSECUTIVE zero-runs' lengths - the score for "sacrifice the single
// one-run between these two zero-runs, merging them into one".
//
// That adjacent-pair maximum alone is a complete answer: filling some OTHER
// zero-run using a smaller sacrifice found elsewhere is never better, because
// whichever one-run borders the single largest zero-run anywhere is itself a
// valid sacrifice, and pairing it with that neighbor already scores at least
// "largest zero-run + 1", which beats "largest zero-run minus any smaller
// sacrifice" outright. A witness meaningful only to this problem (§17.3), so
// it lives in its own LeetCode/ folder rather than Domain/.
internal sealed class ActiveSectionTradeIndex
{
    private readonly string _text;
    private readonly (int Start, int Length)[] _zeroRuns;
    private readonly int[] _zeroRunAtOrBefore;
    private readonly SegmentTree<int, MaxOperation<int>>? _adjacentPairMax;

    public int ActiveOnes { get; }

    public ActiveSectionTradeIndex(string text)
    {
        _text = text;
        ActiveOnes = text.Count(c => c == '1');

        var (zeroRuns, zeroRunAtOrBefore) = BuildZeroRunIndex(text);
        _zeroRuns = zeroRuns;
        _zeroRunAtOrBefore = zeroRunAtOrBefore;

        _adjacentPairMax = BuildAdjacentPairMax(zeroRuns);
    }

    // Every maximal run of '0's in text, plus a per-position pointer to the most
    // recent zero-run starting at or before that position (-1 while text has
    // produced none yet).
    private static ((int Start, int Length)[] ZeroRuns, int[] RunAtOrBefore) BuildZeroRunIndex(string text)
    {
        var zeroRuns = new List<(int Start, int Length)>();
        var zeroRunAtOrBefore = new int[text.Length];

        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '0')
            {
                if (i > 0 && text[i - 1] == '0')
                {
                    var last = zeroRuns[^1];
                    zeroRuns[^1] = (last.Start, last.Length + 1);
                }
                else
                {
                    zeroRuns.Add((i, 1));
                }
            }

            zeroRunAtOrBefore[i] = zeroRuns.Count - 1;
        }

        return ([.. zeroRuns], zeroRunAtOrBefore);
    }

    // The score for sacrificing the single one-run between each pair of CONSECUTIVE
    // zero-runs, as a range-maximum tree over those pairwise sums - or null when
    // text holds fewer than two runs, leaving no pair to merge.
    private static SegmentTree<int, MaxOperation<int>>? BuildAdjacentPairMax((int Start, int Length)[] zeroRuns)
    {
        if (zeroRuns.Length < 2)
        {
            return null;
        }

        var adjacentPairSums = new int[zeroRuns.Length - 1];

        for (var i = 0; i < adjacentPairSums.Length; i++)
        {
            adjacentPairSums[i] = zeroRuns[i].Length + zeroRuns[i + 1].Length;
        }

        return new SegmentTree<int, MaxOperation<int>>(adjacentPairSums);
    }

    // The best achievable active-section count after at most one trade
    // confined to text[left..right] - active elsewhere in text (a trade never
    // touches anything outside [left, right]) plus the best gain such a
    // trade can achieve there (0 when no trade helps).
    public int BestActiveAfterTrade(int left, int right)
    {
        if (_zeroRuns.Length == 0)
        {
            return ActiveOnes;
        }

        var window = ClipToWindow(left, right);

        var interiorGain = InteriorPairGain(window.InteriorStart, window.InteriorEnd);
        var clippedGain = ClippedEndGain(left, right, window.LeftRemainder, window.RightRemainder);

        return ActiveOnes + Math.Max(interiorGain, clippedGain);
    }

    // What the query window sees of the zero-run index: the surviving
    // (query-clipped) tail of the zero-run straddling `left`, and head of the one
    // straddling `right` - only meaningful when text[left]/text[right] is itself '0',
    // guarded at every use. The interior bounds are the zero-run indices fully
    // inside (left, right), neither clipped by the window, so adjacent pairs among
    // them are valid sacrifice-and-merge candidates with no boundary special-casing.
    private (int LeftRemainder, int RightRemainder, int InteriorStart, int InteriorEnd) ClipToWindow(int left, int right)
    {
        var zeroRunAtLeft = _zeroRunAtOrBefore[left];
        var zeroRunAtRight = _zeroRunAtOrBefore[right];

        var leftRemainder = zeroRunAtLeft < 0
            ? 0
            : ClippedTailLength(_zeroRuns[zeroRunAtLeft], left);
        var rightRemainder = zeroRunAtRight < 0
            ? 0
            : ClippedHeadLength(_zeroRuns[zeroRunAtRight], right);
        var rightIsZero = _text[right] == '0';

        return (leftRemainder, rightRemainder, zeroRunAtLeft + 1, zeroRunAtRight - (rightIsZero ? 1 : 0));
    }

    // The query-clipped tail of the zero-run straddling `left`: the run's own length
    // less the positions the window cut off its front.
    private static int ClippedTailLength((int Start, int Length) run, int left) =>
        run.Length - (left - run.Start);

    // The query-clipped head of the zero-run straddling `right`: the run's start
    // through `right` inclusive.
    private static int ClippedHeadLength((int Start, int Length) run, int right) =>
        right - run.Start + 1;

    // The best merge wholly inside the window: the largest pairwise sum among
    // consecutive interior zero-runs, or 0 when the window holds no such pair.
    private int InteriorPairGain(int interiorStart, int interiorEnd)
    {
        if (_adjacentPairMax is null || interiorStart >= interiorEnd)
        {
            return 0;
        }

        return _adjacentPairMax.Query(interiorStart, interiorEnd - 1);
    }

    // The best merge that uses a query-clipped end: both ends clipped with nothing
    // but a single one-run between them, or one clipped end paired with the
    // neighbouring (fully interior) zero-run.
    private int ClippedEndGain(int left, int right, int leftRemainder, int rightRemainder)
    {
        var zeroRunAtLeft = _zeroRunAtOrBefore[left];
        var zeroRunAtRight = _zeroRunAtOrBefore[right];
        var rightIsOne = _text[right] == '1';
        var best = 0;

        if (HasAdjacentClippedEnds(_text[left], _text[right], zeroRunAtLeft, zeroRunAtRight))
        {
            best = Math.Max(best, leftRemainder + rightRemainder);
        }

        if (_text[left] == '0' && zeroRunAtLeft + 1 < zeroRunAtRight + (rightIsOne ? 1 : 0))
        {
            best = Math.Max(best, leftRemainder + _zeroRuns[zeroRunAtLeft + 1].Length);
        }

        if (_text[right] == '0' && zeroRunAtLeft < zeroRunAtRight - 1)
        {
            best = Math.Max(best, rightRemainder + _zeroRuns[zeroRunAtRight - 1].Length);
        }

        return best;
    }

    // Both query ends sit on a '0' and their zero-runs are neighbours, so a single
    // one-run is all that separates the two clipped remainders.
    private static bool HasAdjacentClippedEnds(
        char leftChar, char rightChar, int leftZeroRun, int rightZeroRun) =>
        leftChar == '0' && rightChar == '0' && leftZeroRun + 1 == rightZeroRun;
}
