using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

// LeetCode 3845. Maximum Subarray XOR with Bounded Range: among every non-empty
// subarray whose largest and smallest elements differ by at most k (maxSpread
// here), report the largest XOR of its elements. A single element always
// qualifies (k >= 0), so there is always an answer.
internal static class MaximumSubarrayXORWithBoundedRangeSolution
{
    // Bit 31 is the most significant bit of a 32-bit int; the greedy walks below
    // run MSB first over all 32 levels, as BitTrie itself lays its paths out.
    private const int MostSignificantBitIndex = 31;

    // The textbook O(n^2) scan: from every start index, extend the end one element
    // at a time, keeping the running max, min and XOR, until the spread passes
    // maxSpread. Deliberately written without this repo's primitives - the arm the
    // composed strategy below has to beat.
    public static int MaxSubarrayXorByBruteForce(int[] nums, int maxSpread)
    {
        var best = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var bestFromStart = BestXorStartingAt(nums, start, maxSpread);
            best = Math.Max(best, bestFromStart);
        }

        return best;
    }

    private static int BestXorStartingAt(int[] nums, int start, int maxSpread)
    {
        var (max, min) = (nums[start], nums[start]);
        var runningXor = 0;
        var best = 0;

        for (var end = start; end < nums.Length; end++)
        {
            max = Math.Max(max, nums[end]);
            min = Math.Min(min, nums[end]);

            if (max - min > maxSpread)
            {
                break;
            }

            runningXor ^= nums[end];
            best = Math.Max(best, runningXor);
        }

        return best;
    }

    // Composed: dropping an element from a subarray can only shrink its spread, so
    // for each right edge the valid left edges form one run [left, right] whose
    // left end only ever moves right - the window
    // LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution keeps
    // with two monotonic Deques. A subarray [l, r]'s XOR is prefix[r + 1] XOR
    // prefix[l], so the best subarray ending at r is the best XOR of prefix[r + 1]
    // against the live prefixes prefix[left..r] - one greedy walk down this repo's
    // BitTrie. BitTrie has no Remove, so the prefixes the left edge passes are
    // retired with the subtree-count technique MaximumGeneticDifferenceQuery uses:
    // each trie node counts the live values through it, and the walk only takes a
    // child that still has one. O(n * 32) against the scan's O(n * window).
    public static int MaxSubarrayXorBySlidingWindowBitTrie(int[] nums, int maxSpread)
    {
        var prefix = PrefixXors(nums);
        var live = new LivePrefixes(new BitTrie(), new HashMap<BitTrieNode, int>());
        var window = new MinMaxWindow(nums, maxSpread);
        var left = 0;
        var best = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            Insert(live, prefix[right]);
            var newLeft = window.Advance(right);

            for (; left < newLeft; left++)
            {
                Remove(live, prefix[left]);
            }

            var bestEndingHere = MaxXorAmongLive(live, prefix[right + 1]);
            best = Math.Max(best, bestEndingHere);
        }

        return best;
    }

    // prefix[i] is the XOR of nums[0..i), so prefix has one more entry than nums.
    private static int[] PrefixXors(int[] nums)
    {
        var prefix = new int[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] ^ nums[i];
        }

        return prefix;
    }

    private static void Remove(LivePrefixes live, int value) => AdjustSubtreeCounts(live, value, delta: -1);

    // BitTrie.TryMaxXor's greedy "prefer the opposite bit" walk, restricted to
    // children that still carry a live value. The window always holds at least
    // prefix[right], so some child is live at every level.
    private static int MaxXorAmongLive(LivePrefixes live, int value)
    {
        BitTrieNode? current = live.Trie.Root;
        var bits = unchecked((uint)value);
        var xor = 0u;

        for (var i = MostSignificantBitIndex; i >= 0 && current is not null; i--)
        {
            var (next, matchedOpposite) = DescendToLiveChild(current, live.SubtreeCount, (bits >> i) & 1u);

            if (matchedOpposite)
            {
                xor |= 1u << i;
            }

            current = next;
        }

        return unchecked((int)xor);
    }

    private static (BitTrieNode? Next, bool MatchedOpposite) DescendToLiveChild(
        BitTrieNode current, HashMap<BitTrieNode, int> subtreeCount, uint bit)
    {
        var opposite = ChildForBit(current, bit == 0 ? 1u : 0u);

        if (HasLiveValues(opposite, subtreeCount))
        {
            return (opposite, true);
        }

        return (ChildForBit(current, bit), false);
    }

    private static bool HasLiveValues(BitTrieNode? child, HashMap<BitTrieNode, int> subtreeCount) =>
        child is not null && subtreeCount.TryGetValue(child, out var count) && count > 0;

    private static void Insert(LivePrefixes live, int value)
    {
        live.Trie.Insert(value);
        AdjustSubtreeCounts(live, value, delta: 1);
    }

    // Follows the value's bit path from the root, bumping each visited node's live
    // count by delta. A matching Insert laid every node on the path down first, so
    // the walk always runs the full 32 levels and the null test only answers the
    // compiler.
    private static void AdjustSubtreeCounts(LivePrefixes live, int value, int delta)
    {
        var current = live.Trie.Root;
        var bits = unchecked((uint)value);

        for (var i = MostSignificantBitIndex; i >= 0; i--)
        {
            var child = ChildForBit(current, (bits >> i) & 1u);

            if (child is null)
            {
                return;
            }

            live.SubtreeCount.TryGetValue(child, out var existing);
            live.SubtreeCount.Set(child, existing + delta);
            current = child;
        }
    }

    private static BitTrieNode? ChildForBit(BitTrieNode node, uint bit) =>
        bit == 0 ? node.Zero : node.One;

    // The trie and its per-node live counts, always used together.
    private readonly record struct LivePrefixes(BitTrie Trie, HashMap<BitTrieNode, int> SubtreeCount);

    // Two monotonic index Deques over the same window - one decreasing in value (its
    // front is the window's max), one increasing (its front is the min) - exactly as
    // LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution keeps
    // them. Each index enters and leaves each deque at most once.
    private sealed class MinMaxWindow(int[] nums, int maxSpread)
    {
        private readonly RepoDeque _maxWindow = new();
        private readonly RepoDeque _minWindow = new();
        private int _left;

        // Admits index right, restores the spread bound, and reports the window's
        // new left edge.
        public int Advance(int right)
        {
            PushMax(right);
            PushMin(right);

            while (IsSpreadOverBound())
            {
                _left++;
                DropStaleFronts();
            }

            return _left;
        }

        private void PushMax(int right)
        {
            while (_maxWindow.TryPeekBack(out var maxBack) && nums[maxBack] <= nums[right])
            {
                _maxWindow.TryPopBack(out _);
            }

            _maxWindow.PushBack(right);
        }

        private void PushMin(int right)
        {
            while (_minWindow.TryPeekBack(out var minBack) && nums[minBack] >= nums[right])
            {
                _minWindow.TryPopBack(out _);
            }

            _minWindow.PushBack(right);
        }

        private bool IsSpreadOverBound() =>
            _maxWindow.TryPeekFront(out var maxFront) && _minWindow.TryPeekFront(out var minFront) &&
            nums[maxFront] - nums[minFront] > maxSpread;

        // The left edge advances by one, so at most one index per deque falls behind it.
        private void DropStaleFronts()
        {
            if (_maxWindow.TryPeekFront(out var maxFront) && maxFront < _left)
            {
                _maxWindow.TryPopFront(out _);
            }

            if (_minWindow.TryPeekFront(out var minFront) && minFront < _left)
            {
                _minWindow.TryPopFront(out _);
            }
        }
    }
}
