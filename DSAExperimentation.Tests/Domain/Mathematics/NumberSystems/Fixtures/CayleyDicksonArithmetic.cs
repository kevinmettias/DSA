namespace DSAExperimentation.Tests.Domain.Mathematics.NumberSystems.Fixtures;

// The Cayley-Dickson construction as exact arithmetic on integer coordinates, so the laws the table
// claims can be checked rather than trusted. An element of the k-th doubling of the reals is 2^k
// coordinates, its first half and second half a pair (a, b) of the system before, multiplied as
// (a, b)(c, d) = (ac - d*b, da + bc*), with conjugation (a, b)* = (a*, -b) and a real its own
// conjugate. Coordinate 0 is the real part and coordinate 1 the unit i.
internal static class CayleyDicksonArithmetic
{
    public static long[] Basis(int dimension, int index)
    {
        var element = new long[dimension];
        element[index] = 1;
        return element;
    }

    public static long[] Multiply(long[] left, long[] right)
    {
        if (left.Length == 1)
        {
            return [left[0] * right[0]];
        }

        var half = left.Length / 2;
        var pair = new Halves(left[..half], left[half..], right[..half], right[half..]);
        return [.. FirstHalfOf(pair), .. SecondHalfOf(pair)];
    }

    public static long[] Add(long[] left, long[] right) => [.. left.Zip(right, (x, y) => x + y)];

    public static long[] Subtract(long[] left, long[] right) => [.. left.Zip(right, (x, y) => x - y)];

    // (xy)z - x(yz), zero exactly when the three associate.
    public static long[] Associator(long[] first, long[] second, long[] third)
    {
        var leftFirst = Multiply(Multiply(first, second), third);
        var rightFirst = Multiply(first, Multiply(second, third));
        return Subtract(leftFirst, rightFirst);
    }

    public static long NormSquared(long[] element) => element.Sum(coordinate => coordinate * coordinate);

    public static bool IsZero(long[] element) => element.All(coordinate => coordinate == 0);

    // ac - d*b
    private static long[] FirstHalfOf(Halves pair)
    {
        var product = Multiply(pair.A, pair.C);
        var correction = Multiply(Conjugate(pair.D), pair.B);
        return Subtract(product, correction);
    }

    // da + bc*
    private static long[] SecondHalfOf(Halves pair)
    {
        var product = Multiply(pair.D, pair.A);
        var correction = Multiply(pair.B, Conjugate(pair.C));
        return Add(product, correction);
    }

    private static long[] Conjugate(long[] element)
    {
        if (element.Length == 1)
        {
            return [element[0]];
        }

        var half = element.Length / 2;
        var negatedSecond = element[half..].Select(coordinate => -coordinate);
        return [.. Conjugate(element[..half]), .. negatedSecond];
    }

    // The halves of two elements (a, b) and (c, d) about to be multiplied.
    private readonly record struct Halves(long[] A, long[] B, long[] C, long[] D);
}
