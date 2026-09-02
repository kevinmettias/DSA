using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignMovieRentalSystem;

// LeetCode 1912. Design Movie Rental System: search()'s "5 cheapest unrented shops
// for a movie, price then shop ascending" and report()'s "5 cheapest rented
// (shop,movie) pairs, price then shop then movie ascending" are both exactly what a
// BST's in-order walk already produces for a ValueTuple key (ValueTuple.CompareTo is
// element-wise, so ordering by (Price, Shop[, Movie]) falls straight out of the
// default IComparable<TValue> ordering) - one BinarySearchTree<(int,int)> per movie
// (this repo's own BinarySearchTree/InOrderTraversal, the same composition
// KthSmallestElementInABSTTests/AllElementsInTwoBinarySearchTreesTests use) tracks
// that movie's unrented copies, and a second BinarySearchTree<(int,int,int)> tracks
// every currently rented copy globally; renting/dropping just moves one
// (Price, Shop[, Movie]) entry between the two trees. Prices are looked up through
// HashMap<(int,int),int> keyed by (shop, movie).
public sealed partial class DesignMovieRentalSystemTests
{
    [Fact]
    public void MovieRentingSystem_LeetCodeExample_TracksSearchRentDropAndReport()
    {
        int[][] entries =
        [
            [0, 1, 5],
            [0, 2, 6],
            [0, 3, 7],
            [1, 1, 4],
            [1, 2, 7],
            [2, 1, 5],
        ];
        var system = new MovieRentingSystem(3, entries);

        Assert.Equal([1, 0, 2], system.Search(1));

        system.Rent(0, 1);
        system.Rent(1, 2);

        Assert.Equal([[0, 1], [1, 2]], system.Report());

        system.Drop(1, 2);

        Assert.Equal([0, 1], system.Search(2));
    }

    [Fact]
    public void Search_MovieWithNoEntries_ReturnsEmptyList()
    {
        int[][] entries = [[0, 1, 5]];
        var system = new MovieRentingSystem(1, entries);

        Assert.Empty(system.Search(99));
    }

    [Fact]
    public void Search_MoreThanFiveUnrentedShops_ReturnsOnlyFiveCheapest()
    {
        int[][] entries =
        [
            [0, 1, 6], [1, 1, 5], [2, 1, 4], [3, 1, 3], [4, 1, 2], [5, 1, 1],
        ];
        var system = new MovieRentingSystem(6, entries);

        Assert.Equal([5, 4, 3, 2, 1], system.Search(1));
    }

    private sealed class MovieRentingSystem
    {
        private const int ResultCap = 5;

        private readonly HashMap<(int Shop, int Movie), int> _priceOf = new();
        private readonly HashMap<int, BinarySearchTree<(int Price, int Shop)>> _unrentedByMovie = new();
        private readonly BinarySearchTree<(int Price, int Shop, int Movie)> _rented = new();

        public MovieRentingSystem(int shopCount, int[][] entries)
        {
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
}
