using DSAExperimentation.Domain.Mathematics.Algebra;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.Solutions.DesignSkiplist;

// LeetCode 1206. Design Skiplist: a multiset of ints supporting Add/Search/Erase
// without using any built-in ordered container, where Erase reports whether it
// actually removed an occurrence.
//
// An instance API rather than a pure function, so "every strategy for the problem"
// (§17.3) takes the form of two classes implementing the shared ISkiplist surface
// below instead of two static methods sharing an <Operation>By<Strategy> name - the
// same shape DesignHashMapSolution already uses for its own Design-category problem.
// There is no separate "prepare input" step to hoist into a benchmark's
// [GlobalSetup]; each arm constructs its own instance and replays the same call
// script.
//
// The observation that makes the Fenwick arm possible: add/erase/search only ever
// need "how many of this exact value are currently stored", never an ordered walk
// between values, so no probabilistic multi-level list is needed at all.
[LeetCodeQuestion("design-skiplist", LeetCodeAnswerOrder.AsWritten)]
internal static class DesignSkiplistSolution
{
    // Each strategy as LeetCode's constructor makes it, so the catalog harness can make the object its call
    // scripts run on (ARCHITECTURE 17.13).
    public static ISkiplist CreateByLinearScanList() => new SkiplistByLinearScanList();

    public static ISkiplist CreateByFenwickFrequencies() => new SkiplistByFenwickFrequencies();

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ISkiplist
    {
        void Add(int num);

        bool Search(int target);

        bool Erase(int num);
    }

    // The textbook answer: a plain BCL List<int> used as an unordered multiset -
    // Contains for Search, IndexOf + RemoveAt for Erase, both O(n). Deliberately
    // written without this repo's primitives; it is the arm the frequency-tree
    // strategy below has to justify itself against.
    internal sealed class SkiplistByLinearScanList : ISkiplist
    {
        private readonly List<int> _values = [];

        public void Add(int num) => _values.Add(num);

        public bool Search(int target) => _values.Contains(target);

        public bool Erase(int num)
        {
            var index = _values.IndexOf(num);

            if (index < 0)
            {
                return false;
            }

            _values.RemoveAt(index);
            return true;
        }
    }

    // This repo's own FenwickTree<int, Addition<int>>, used as a point-update /
    // point-query frequency array over num's bounded domain (0 <= num <= 2*10^4 per
    // LeetCode's own constraint). Add is FenwickTree's native +1 delta; Search and
    // Erase both read the current count through a single-index Query, and Erase
    // turns that into a -1 delta. Every call is O(log 2*10^4).
    internal sealed class SkiplistByFenwickFrequencies : ISkiplist
    {
        private readonly FenwickTree<int, Addition<int>> _frequencies = new(DesignSkiplistConstants.MaxValue + 1);

        public void Add(int num) => _frequencies.Add(num, 1);

        public bool Search(int target) => _frequencies.Query(target, target) > 0;

        public bool Erase(int num)
        {
            if (!Search(num))
            {
                return false;
            }

            _frequencies.Add(num, -1);
            return true;
        }
    }
}
