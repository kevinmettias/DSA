using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.DirectionAssignmentsWithExactlyKVisiblePeople;

// LeetCode 3881. Direction Assignments with Exactly K Visible People: the
// person at pos sees exactly the left-segment people who chose 'L' plus the
// right-segment people who chose 'R'; their own choice never affects the
// count. Left segment size is pos, right segment size is n - 1 - pos, and
// every choice is independent, so the answer is 2 (the free choice at pos)
// times the number of ways to split k "visible" votes between the two
// segments: sum over a of C(pos, a) * C(n - 1 - pos, k - a).
internal static class DirectionAssignmentsWithExactlyKVisiblePeopleSolution
{
    // Textbook baseline: build each segment's binomial-coefficient row with
    // Pascal's triangle addition alone (no modular inverse), then convolve the
    // two rows directly - the sum-over-a form before it is recognized as an
    // instance of Vandermonde's identity. O(pos^2 + rightSize^2) to build the
    // rows, the arm the closed form below has to beat.
    public static int CountAssignmentsByPascalConvolution(int n, int pos, int k)
    {
        var rightSize = n - 1 - pos;
        var leftRow = BuildBinomialRow(pos);
        var rightRow = BuildBinomialRow(rightSize);

        var lowA = Math.Max(0, k - rightSize);
        var highA = Math.Min(pos, k);
        var total = 0L;

        for (var a = lowA; a <= highA; a++)
        {
            total = (total + leftRow[a] * rightRow[k - a]) % ModularArithmetic.Modulo;
        }

        return (int)(total * 2 % ModularArithmetic.Modulo);
    }

    private static long[] BuildBinomialRow(int size)
    {
        var row = new long[size + 1];
        row[0] = 1;

        for (var person = 1; person <= size; person++)
        {
            for (var a = person; a >= 1; a--)
            {
                row[a] = (row[a] + row[a - 1]) % ModularArithmetic.Modulo;
            }
        }

        return row;
    }

    // Vandermonde's identity: sum_a C(pos, a) * C(rightSize, k - a) = C(pos +
    // rightSize, k) = C(n - 1, k), since pos + rightSize is everyone except the
    // person at pos. So the whole convolution collapses to one binomial
    // coefficient, doubled for pos's own free choice. Domain.Modular supplies
    // the Fermat's-little-theorem inverse the factorial table needs
    // (RoomWaysPrecomputedFactorialAlgebra / CountNumberOfBalancedPermutations
    // precedent for building factorial/inverse-factorial tables from it).
    public static int CountAssignmentsByVandermondeIdentity(int n, int pos, int k)
    {
        var (factorial, inverseFactorial) = BuildFactorialTable(n - 1);
        var combinations = Combination(n - 1, k, factorial, inverseFactorial);

        return (int)(combinations * 2 % ModularArithmetic.Modulo);
    }

    private static (long[] Factorial, long[] InverseFactorial) BuildFactorialTable(int maxSize)
    {
        var factorial = new long[maxSize + 1];
        var inverseFactorial = new long[maxSize + 1];
        factorial[0] = 1;

        for (var i = 1; i <= maxSize; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        inverseFactorial[maxSize] = ModularArithmetic.Inverse(factorial[maxSize]);

        for (var i = maxSize - 1; i >= 0; i--)
        {
            inverseFactorial[i] = inverseFactorial[i + 1] * (i + 1) % ModularArithmetic.Modulo;
        }

        return (factorial, inverseFactorial);
    }

    private static long Combination(int total, int choose, long[] factorial, long[] inverseFactorial)
    {
        if (choose < 0 || choose > total)
        {
            return 0;
        }

        return factorial[total] * inverseFactorial[choose] % ModularArithmetic.Modulo *
            inverseFactorial[total - choose] % ModularArithmetic.Modulo;
    }
}
