using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheKthLargestIntegerInTheArray;

// LeetCode 1985. Find the Kth Largest Integer in the Array: nums holds arbitrarily large
// non-negative integers as digit strings, too big for long, so "largest" can't use string's own
// IComparable<string> (lexicographic - wrong once lengths differ, e.g. "9" > "10"). NumericStringToken
// wraps each string in the correct numeric ordering (longer digit string wins; same length falls
// back to ordinal digit-by-digit compare), then this repo's own Heap<Element,MinHeapOrder<Element>>
// runs the exact size-k min-heap KthLargestElementTests already establishes for LC215 - just over
// this problem-local IComparable<T>, not int.
public sealed partial class FindTheKthLargestIntegerInTheArrayTests
{
    [Fact]
    public void KthLargestNumber_ClassicExample_ReturnsThirdLargestByValue()
    {
        string[] nums = ["3", "6", "7", "10"];

        var result = KthLargestNumber(nums, k: 4);

        Assert.Equal("3", result);
    }

    [Fact]
    public void KthLargestNumber_SameLengthStrings_ComparesDigitByDigitNotLexicographically()
    {
        string[] nums = ["2", "21", "12", "1"];

        var result = KthLargestNumber(nums, k: 3);

        Assert.Equal("2", result);
    }

    [Fact]
    public void KthLargestNumber_LongerStringOutranksShorterRegardlessOfLeadingDigit()
    {
        string[] nums = ["1", "9", "10", "2"];

        var result = KthLargestNumber(nums, k: 1);

        Assert.Equal("10", result);
    }

    private static string KthLargestNumber(string[] nums, int k)
    {
        var heap = new Heap<NumericStringToken, MinHeapOrder<NumericStringToken>>();

        foreach (var num in nums)
        {
            heap.Push(new NumericStringToken(num));

            if (heap.Count > k)
            {
                heap.TryPop(out _);
            }
        }

        heap.TryPeek(out var kthLargest);
        return kthLargest.Value;
    }

    private readonly record struct NumericStringToken(string Value) : IComparable<NumericStringToken>
    {
        public int CompareTo(NumericStringToken other) => Value.Length != other.Value.Length
            ? Value.Length.CompareTo(other.Value.Length)
            : string.CompareOrdinal(Value, other.Value);
    }
}
