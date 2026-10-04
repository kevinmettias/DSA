using DSAExperimentation.DataStructures.CountedBitTrie;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

// LeetCode 3845. Maximum Subarray XOR with Bounded Range: among every non-empty
// subarray whose largest and smallest elements differ by at most k (maxSpread
// here), report the largest XOR of its elements. A single element always
// qualifies (k >= 0), so there is always an answer.
internal static class MaximumSubarrayXORWithBoundedRangeSolution
{
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
    // CountedBitTrie, which takes back each prefix the left edge passes. The window
    // always holds prefix[right], so that walk always finds a live value.
    // O(n * 32) against the scan's O(n * window).
    public static int MaxSubarrayXorBySlidingWindowBitTrie(int[] nums, int maxSpread)
    {
        var prefix = PrefixXors(nums);
        var live = new CountedBitTrie();
        var window = new MinMaxWindow(nums, maxSpread);
        var left = 0;
        var best = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            live.Insert(prefix[right]);
            var newLeft = window.Advance(right);

            for (; left < newLeft; left++)
            {
                live.TryRemove(prefix[left]);
            }

            live.TryMaxXor(prefix[right + 1], out var bestEndingHere);
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
