using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DesignMovieRentalSystem;

// LeetCode 1912. Design Movie Rental System: an instance API (search/rent/drop/
// report) rather than a pure function, so - as with DesignAuctionSystemSolution -
// "every strategy for the problem" (ARCHITECTURE.md 17.3) takes the form of two
// full classes behind the shared IMovieRentingSystem surface below, and the
// harnesses replay one call script against either of them.
//
// Both of LeetCode's queries are "the 5 cheapest, ties broken by shop (then by
// movie)", which is a total order on the tuple (Price, Shop[, Movie]) - and
// ValueTuple.CompareTo is already element-wise, so that ordering falls straight
// out of the default IComparable ordering of the tuple. What the two strategies
// disagree about is when the ordering is paid for: per query, or maintained.
internal static class DesignMovieRentalSystemSolution
{
    // LeetCode caps both search() and report() at five results.
    private const int ResultCap = 5;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either without restating it.
    internal interface IMovieRentingSystem
    {
        List<int> Search(int movie);

        void Rent(int shop, int movie);

        void Drop(int shop, int movie);

        List<List<int>> Report();
    }

    // The textbook answer: every (shop, movie, price) entry in one flat list plus a
    // set of the rented pairs, and each query filters and sorts that list from
    // scratch - O(m log m) per search() and per report(). Deliberately BCL-only,
    // no primitive from this repo; it is the arm the maintained-order strategy
    // below has to justify itself against.
    //
    // Pre-migration this arm lived in the benchmark and answered search() alone,
    // with no rent/drop/report at all - promoting it to the full LeetCode API is
    // what puts it under test.
    internal sealed class MovieRentingSystemBySortOnQuery : IMovieRentingSystem
    {
        private readonly List<(int Shop, int Movie, int Price)> _entries;
        private readonly HashSet<(int Shop, int Movie)> _rented = [];

        // shopCount is LeetCode's n, an upper bound on the shop ids; neither
        // strategy needs it, because both key off the ids the entries carry.
        public MovieRentingSystemBySortOnQuery(int shopCount, int[][] entries)
        {
            _ = shopCount;
            _entries = entries.Select(entry => (entry[0], entry[1], entry[2])).ToList();
        }

        public List<int> Search(int movie)
            => _entries
                .Where(entry => entry.Movie == movie && !_rented.Contains((entry.Shop, entry.Movie)))
                .OrderBy(entry => entry.Price)
                .ThenBy(entry => entry.Shop)
                .Take(ResultCap)
                .Select(entry => entry.Shop)
                .ToList();

        public void Rent(int shop, int movie) => _rented.Add((shop, movie));

        public void Drop(int shop, int movie) => _rented.Remove((shop, movie));

        public List<List<int>> Report()
            => _entries
                .Where(entry => _rented.Contains((entry.Shop, entry.Movie)))
                .OrderBy(entry => entry.Price)
                .ThenBy(entry => entry.Shop)
                .ThenBy(entry => entry.Movie)
                .Take(ResultCap)
                .Select(entry => new List<int> { entry.Shop, entry.Movie })
                .ToList();
    }

    // This repo's own BinarySearchTree/InOrderTraversal composition - the same one
    // KthSmallestElementInABSTSolution and AllElementsInTwoBinarySearchTreesSolution
    // use. One tree of (Price, Shop) per movie holds that movie's unrented copies
    // and a single tree of (Price, Shop, Movie) holds every rented copy, so both
    // queries are a prefix of an in-order walk and rent/drop just move one entry
    // between the two trees. Prices are recovered from a HashMap keyed by
    // (shop, movie).
    internal sealed class MovieRentingSystemByBinarySearchTree : IMovieRentingSystem
    {
        private readonly HashMap<(int Shop, int Movie), int> _priceOf = new();
        private readonly HashMap<int, BinarySearchTree<(int Price, int Shop)>> _unrentedByMovie = new();
        private readonly BinarySearchTree<(int Price, int Shop, int Movie)> _rented = new();

        public MovieRentingSystemByBinarySearchTree(int shopCount, int[][] entries)
        {
            _ = shopCount;

            foreach (var entry in entries)
            {
                var (shop, movie, price) = (entry[0], entry[1], entry[2]);
                _priceOf.Set((shop, movie), price);
                UnrentedTreeFor(movie).Insert((price, shop));
            }
        }

        public List<int> Search(int movie)
        {
            var results = new List<int>();

            if (!_unrentedByMovie.TryGetValue(movie, out var tree))
            {
                return results;
            }

            foreach (var entry in CollectAscending(tree.Root, ResultCap))
            {
                results.Add(entry.Shop);
            }

            return results;
        }

        public void Rent(int shop, int movie)
        {
            _priceOf.TryGetValue((shop, movie), out var price);
            UnrentedTreeFor(movie).TryDelete((price, shop));
            _rented.Insert((price, shop, movie));
        }

        public void Drop(int shop, int movie)
        {
            _priceOf.TryGetValue((shop, movie), out var price);
            _rented.TryDelete((price, shop, movie));
            UnrentedTreeFor(movie).Insert((price, shop));
        }

        public List<List<int>> Report()
        {
            var results = new List<List<int>>();

            foreach (var entry in CollectAscending(_rented.Root, ResultCap))
            {
                results.Add([entry.Shop, entry.Movie]);
            }

            return results;
        }

        private BinarySearchTree<(int Price, int Shop)> UnrentedTreeFor(int movie)
        {
            if (!_unrentedByMovie.TryGetValue(movie, out var tree))
            {
                tree = new BinarySearchTree<(int, int)>();
                _unrentedByMovie.Set(movie, tree);
            }

            return tree;
        }
    }

    // In-order visits a BST's values in ascending order, so the first cap of them
    // are the answer. IInOrderHooks.Visit is static and has no early-exit signal
    // (see InOrderTraversal's own doc note), so the walk covers the whole tree and
    // the cap only bounds what is kept - the same arrangement
    // KthSmallestElementInABSTSolution uses, with the buffer in AsyncLocal state
    // alongside the walk.
    private static List<TValue> CollectAscending<TValue>(BinaryTreeNode<TValue>? root, int cap)
        where TValue : IComparable<TValue>
    {
        CollectState<TValue>.Values.Value = new List<TValue>();
        CollectState<TValue>.Cap.Value = cap;
        InOrderTraversal.Walk<TValue, CollectHooks<TValue>>(root);
        return CollectState<TValue>.Values.Value;
    }

    private readonly struct CollectHooks<TValue> : IInOrderHooks<TValue>
        where TValue : IComparable<TValue>
    {
        public static void Visit(BinaryTreeNode<TValue> node, int depth)
        {
            var values = CollectState<TValue>.Values.Value!;

            if (values.Count < CollectState<TValue>.Cap.Value)
            {
                values.Add(node.Value);
            }
        }
    }

    private static class CollectState<TValue>
    {
        public static readonly AsyncLocal<List<TValue>> Values = new();
        public static readonly AsyncLocal<int> Cap = new();
    }
}
