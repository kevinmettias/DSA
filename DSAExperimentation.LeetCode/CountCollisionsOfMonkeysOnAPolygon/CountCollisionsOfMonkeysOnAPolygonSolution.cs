using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountCollisionsOfMonkeysOnAPolygon;

// LeetCode 2550. Count Collisions of Monkeys on a Polygon: each of the n monkeys
// independently moves to one of its two neighbouring vertices, so there are 2^n
// configurations in total; the only two that produce no collision at all are
// "every monkey clockwise" and "every monkey counterclockwise", because any other
// mix leaves two adjacent monkeys meeting on an edge or a vertex. The answer is
// therefore (2^n - 2) modulo 1e9+7.
//
// Both strategies compute exactly that and differ only in how the power is raised:
// once per monkey, or by squaring. No repo container or algorithm primitive
// applies to arithmetic over two running scalars - the same "lighter repo-
// primitive fit" CountGoodNumbersSolution records for its own 5^a * 4^b product.
// The modulus and the squaring loop are Domain.Modular's, because 1e9+7 is
// LeetCode's reporting convention rather than this problem's own value.
internal static class CountCollisionsOfMonkeysOnAPolygonSolution
{
    // Every monkey picks one of its two neighbouring vertices.
    private const long DirectionChoices = 2;

    // All-clockwise and all-counterclockwise, the only two collision-free
    // configurations, are what gets subtracted from the 2^n total.
    private const long CollisionFreeConfigurations = 2;

    // The textbook baseline: double the running count once per monkey, reducing
    // mod 1e9+7 after each multiplication so nothing overflows. Deliberately
    // written without this repo's primitives - it is the O(n) arm the squaring
    // strategy below has to justify itself against, and at LC 2550's own bound of
    // n = 1e9 it is the arm that does not finish.
    public static int NumberOfWaysByRepeatedMultiplication(int n)
    {
        var configurations = NaivePower(DirectionChoices, n);

        return WithoutCollisionFreeConfigurations(configurations);
    }

    private static long NaivePower(long value, long exponent)
    {
        var result = 1L;

        for (var i = 0L; i < exponent; i++)
        {
            result = result * value % ModularArithmetic.Modulo;
        }

        return result;
    }

    // Halve the exponent each step instead of decrementing it, squaring the base to
    // compensate - Domain.Modular's own exponentiation-by-squaring loop, which
    // folds under the modulus at every multiplication so intermediate values never
    // grow past Modulo^2. O(log n) instead of O(n).
    public static int NumberOfWaysByExponentiationBySquaring(int n)
    {
        var configurations = ModularArithmetic.Power(DirectionChoices, n);

        return WithoutCollisionFreeConfigurations(configurations);
    }

    // Subtraction under a modulus can go negative - 2^n mod 1e9+7 is smaller than 2
    // for some n - so the modulus is added back before the final reduction.
    private static int WithoutCollisionFreeConfigurations(long configurations) =>
        (int)((configurations - CollisionFreeConfigurations + ModularArithmetic.Modulo) %
            ModularArithmetic.Modulo);
}
