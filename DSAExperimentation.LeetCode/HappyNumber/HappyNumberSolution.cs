namespace DSAExperimentation.LeetCode.HappyNumber;

// LeetCode 202. Happy Number: repeatedly replace n by the sum of the squares of
// its digits; report whether that process reaches 1 before it repeats a value
// (which proves it has entered a cycle and will never reach 1).
//
// The two strategies differ only in how much space they spend to detect the
// repeat: the baseline remembers every sum it has already produced, while the
// composed strategy runs the same Floyd tortoise-and-hare idea LinkedListCycle
// uses, walking two virtual pointers through the sum-of-squares chain instead of
// a linked list's Next pointers, so it needs none.
internal static class HappyNumberSolution
{
    private const int FirstDigitBase = 10;

    // Textbook baseline: record every digit-square sum produced so far in a BCL
    // HashSet and stop the first time one repeats. O(cycle length) extra space,
    // written without this repo's primitives - the arm the composed strategy has
    // to beat.
    public static bool IsHappyByVisitedSet(int n)
    {
        var visited = new HashSet<int>();

        while (n != 1 && visited.Add(n))
        {
            n = SumOfSquaredDigits(n);
        }

        return n == 1;
    }

    // Floyd tortoise-and-hare over the implicit chain n -> SumOfSquaredDigits(n):
    // a cycle exists (and n is unhappy) exactly when the two pointers meet before
    // either reaches 1. O(1) extra space.
    public static bool IsHappyByFloydCycleDetection(int n)
    {
        var slow = n;
        var fast = SumOfSquaredDigits(n);

        while (fast != 1 && slow != fast)
        {
            slow = SumOfSquaredDigits(slow);
            fast = SumOfSquaredDigits(SumOfSquaredDigits(fast));
        }

        return fast == 1;
    }

    private static int SumOfSquaredDigits(int n)
    {
        var sum = 0;

        while (n > 0)
        {
            var digit = n % FirstDigitBase;
            sum += digit * digit;
            n /= FirstDigitBase;
        }

        return sum;
    }
}
