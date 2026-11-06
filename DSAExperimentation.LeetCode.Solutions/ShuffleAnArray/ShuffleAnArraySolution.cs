using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.Solutions.ShuffleAnArray;

// LeetCode 384. Shuffle an Array: Solution(nums) stores the original configuration
// once; shuffle() returns a uniformly random permutation of it and reset() returns
// to the stored original. reset() is the identity on the caller's own array and
// shuffle() always starts back from the stored original rather than a previous
// shuffle's result, so each strategy's shuffle is a pure function of that array plus
// a Random - no persistent shuffle state is needed, and neither strategy mutates
// the array it is given.
//
// Each strategy is reached two ways under one name: with nums alone it makes
// LeetCode's Solution object, which the catalog harness replays call scripts on
// (ARCHITECTURE 17.13); with nums and a Random it is one shuffle, the algorithm that
// object's shuffle() calls, so each algorithm is written once. LeetCode's answers are
// random, so ShuffleAnArrayAnswerCheck judges them rather than the recorded draw.
[LeetCodeQuestion("shuffle-an-array", LeetCodeAnswerOrder.AsWritten, AnswerCheck = typeof(ShuffleAnArrayAnswerCheck))]
internal static class ShuffleAnArraySolution
{
    // Fixed, so a replay of one script draws the same shuffles every time.
    private const int Seed = 1;

    // LeetCode's Solution(nums), shuffling by removing a random remaining element.
    public static IArrayShuffler ShuffleByRemoveRandomRemaining(int[] nums) => new RemoveRandomRemainingShuffler(nums, Seed);

    // The naive baseline: repeatedly draw a random *remaining* element and remove
    // it from a shrinking pool. BCL List<int>.RemoveAt shifts every trailing
    // element on almost every draw, so this is worst-case O(n^2).
    public static int[] ShuffleByRemoveRandomRemaining(int[] original, Random random)
    {
        var remaining = new List<int>(original);
        var result = new int[remaining.Count];

        for (var i = 0; i < result.Length; i++)
        {
            var index = random.Next(remaining.Count);
            result[i] = remaining[index];
            remaining.RemoveAt(index);
        }

        return result;
    }

    // LeetCode's Solution(nums), shuffling by Fisher-Yates over a DynamicArray<int>.
    public static IArrayShuffler ShuffleByFisherYatesDynamicArray(int[] nums) => new FisherYatesDynamicArrayShuffler(nums, Seed);

    // In-place Fisher-Yates over this repo's own DynamicArray<int>: swap each
    // position against a random earlier-or-equal one, walking from the end down
    // to index 1. O(n), no auxiliary "remaining pool" collection at all.
    public static int[] ShuffleByFisherYatesDynamicArray(int[] original, Random random)
    {
        var values = new DynamicArray<int>();

        foreach (var value in original)
        {
            values.Add(value);
        }

        for (var i = values.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            var temp = values.Get(i);
            values.Set(i, values.Get(j));
            values.Set(j, temp);
        }

        var result = new int[values.Count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = values.Get(i);
        }

        return result;
    }

    // LeetCode's Solution class as its call scripts use it: reset() and shuffle(),
    // each answering with an array of the caller's own.
    internal interface IArrayShuffler
    {
        int[] Reset();

        int[] Shuffle();
    }

    // Keeps its own copy of nums, so a caller rewriting the array it passed, or one
    // an answer handed back, never changes what reset() returns.
    private sealed class RemoveRandomRemainingShuffler(int[] nums, int seed) : IArrayShuffler
    {
        private readonly int[] _original = [.. nums];
        private readonly Random _random = new(seed);

        public int[] Reset() => [.. _original];

        public int[] Shuffle() => ShuffleByRemoveRandomRemaining(_original, _random);
    }

    // The same object over the Fisher-Yates shuffle.
    private sealed class FisherYatesDynamicArrayShuffler(int[] nums, int seed) : IArrayShuffler
    {
        private readonly int[] _original = [.. nums];
        private readonly Random _random = new(seed);

        public int[] Reset() => [.. _original];

        public int[] Shuffle() => ShuffleByFisherYatesDynamicArray(_original, _random);
    }
}
