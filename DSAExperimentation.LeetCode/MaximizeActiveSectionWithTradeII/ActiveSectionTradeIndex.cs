using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MaximizeActiveSectionWithTradeII;

// The static, per-string index MaxActiveAfterTradeByRangeMaxIndex composes
// against: the total '1' count in s (constant across every query - s and the
// query list never change, only which spots a trade touches), every maximal
// run of '0's in s, a per-position pointer to "the most recent zero-run
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
    private readonly string _s;
    private readonly (int Start, int Length)[] _zeroRuns;
    private readonly int[] _zeroRunAtOrBefore;
    private readonly SegmentTree<int, MaxOperation<int>>? _adjacentPairMax;

    public int ActiveOnes { get; }

    public ActiveSectionTradeIndex(string s)
    {
        _s = s;
        ActiveOnes = s.Count(c => c == '1');

        var zeroRuns = new List<(int Start, int Length)>();
        var zeroRunAtOrBefore = new int[s.Length];

        for (var i = 0; i < s.Length; i++)
        {
            if (s[i] == '0')
            {
                if (i > 0 && s[i - 1] == '0')
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

        _zeroRuns = [.. zeroRuns];
        _zeroRunAtOrBefore = zeroRunAtOrBefore;

        if (_zeroRuns.Length >= 2)
        {
            var adjacentPairSums = new int[_zeroRuns.Length - 1];

            for (var i = 0; i < adjacentPairSums.Length; i++)
            {
                adjacentPairSums[i] = _zeroRuns[i].Length + _zeroRuns[i + 1].Length;
            }

            _adjacentPairMax = new SegmentTree<int, MaxOperation<int>>(adjacentPairSums);
        }
    }

    // The best achievable active-section count after at most one trade
    // confined to s[left..right] - active elsewhere in s (a trade never
    // touches anything outside [left, right]) plus the best gain such a
    // trade can achieve there (0 when no trade helps).
    public int BestActiveAfterTrade(int left, int right)
    {
        if (_zeroRuns.Length == 0)
        {
            return ActiveOnes;
        }

        var zeroRunAtLeft = _zeroRunAtOrBefore[left];
        var zeroRunAtRight = _zeroRunAtOrBefore[right];

        // The surviving (query-clipped) tail of the zero-run straddling
        // `left`, and head of the one straddling `right` - only meaningful
        // when s[left]/s[right] is itself '0', guarded at every use below.
        var leftRemainder = zeroRunAtLeft < 0
            ? 0
            : _zeroRuns[zeroRunAtLeft].Length - (left - _zeroRuns[zeroRunAtLeft].Start);
        var rightRemainder = zeroRunAtRight < 0
            ? 0
            : right - _zeroRuns[zeroRunAtRight].Start + 1;

        // Zero-run indices [interiorStart, interiorEnd] are the ones fully
        // inside (left, right) - neither clipped by the query window - so
        // adjacent pairs among them are valid sacrifice-and-merge candidates
        // with no boundary special-casing needed.
        var interiorStart = zeroRunAtLeft + 1;
        var interiorEnd = zeroRunAtRight - (_s[right] == '0' ? 1 : 0);

        var best = ActiveOnes;

        if (_adjacentPairMax is not null && interiorStart < interiorEnd)
        {
            best = Math.Max(best, ActiveOnes + _adjacentPairMax.Query(interiorStart, interiorEnd - 1));
        }

        // Both ends clipped, and nothing but a single one-run separates
        // them: sacrifice that one-run, merge the two remainders.
        if (_s[left] == '0' && _s[right] == '0' && zeroRunAtLeft + 1 == zeroRunAtRight)
        {
            best = Math.Max(best, ActiveOnes + leftRemainder + rightRemainder);
        }

        // Left end clipped: pair its remainder with the next (fully
        // interior) zero-run, sacrificing the one-run between them.
        if (_s[left] == '0' && zeroRunAtLeft + 1 < zeroRunAtRight + (_s[right] == '1' ? 1 : 0))
        {
            best = Math.Max(best, ActiveOnes + leftRemainder + _zeroRuns[zeroRunAtLeft + 1].Length);
        }

        // Right end clipped: symmetric pairing with the previous zero-run.
        if (_s[right] == '0' && zeroRunAtLeft < zeroRunAtRight - 1)
        {
            best = Math.Max(best, ActiveOnes + rightRemainder + _zeroRuns[zeroRunAtRight - 1].Length);
        }

        return best;
    }
}
