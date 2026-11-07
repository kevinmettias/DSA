using DSAExperimentation.Domain.Mathematics.NumberSystems;
using Arithmetic = DSAExperimentation.Tests.Domain.Mathematics.NumberSystems.Fixtures.CayleyDicksonArithmetic;

namespace DSAExperimentation.Tests.Domain.Mathematics.NumberSystems.Fixtures;

// Which laws a Cayley-Dickson algebra's arithmetic keeps, decided from the arithmetic itself. The
// product and the associator are linear in each factor, so commutativity, associativity and
// alternativity hold everywhere exactly when they hold on the basis: alternativity as the
// associator changing sign when either neighbouring pair of factors is swapped, which over the
// integers is the alternative laws. Order fails once a unit squares to -1, a negative square.
// Division is the norm multiplying, |xy|^2 = |x|^2 |y|^2, which rules out zero divisors; that is a
// polynomial identity of degree four, checked on seeded elements rather than proven, and its
// failure is shown by a zero divisor.
internal static class CayleyDicksonLaws
{
    private const int DivisionSamples = 40;
    private const int CoordinateBound = 4;

    private static readonly (AlgebraicLaws Law, Func<int, bool> Holds)[] Deciders =
    [
        (AlgebraicLaws.Order, IsOrdered),
        (AlgebraicLaws.Commutativity, IsCommutative),
        (AlgebraicLaws.Associativity, IsAssociative),
        (AlgebraicLaws.Alternativity, IsAlternative),
        (AlgebraicLaws.Division, IsNormMultiplicative),
    ];

    public static AlgebraicLaws Of(int dimension) =>
        Deciders.Where(decider => decider.Holds(dimension))
            .Aggregate(AlgebraicLaws.None, (laws, decider) => laws | decider.Law);

    private static bool IsOrdered(int dimension) =>
        BasisIndices(dimension).All(index => SquareOfUnit(dimension, index)[0] >= 0);

    private static long[] SquareOfUnit(int dimension, int index)
    {
        var unit = Arithmetic.Basis(dimension, index);
        return Arithmetic.Multiply(unit, unit);
    }

    private static bool IsCommutative(int dimension) =>
        BasisPairs(dimension).All(pair =>
            Arithmetic.Multiply(pair.X, pair.Y).SequenceEqual(Arithmetic.Multiply(pair.Y, pair.X)));

    private static bool IsAssociative(int dimension) =>
        BasisTriples(dimension).All(triple =>
            Arithmetic.IsZero(Arithmetic.Associator(triple.X, triple.Y, triple.Z)));

    private static bool IsAlternative(int dimension) =>
        BasisTriples(dimension).All(triple =>
            IsAlternatingIn(triple.X, triple.Y, triple.Z));

    // [x, y, z] + [y, x, z] = 0 and [z, x, y] + [z, y, x] = 0: the associator changes sign when
    // its first two factors swap, or its last two.
    private static bool IsAlternatingIn(long[] first, long[] second, long[] third)
    {
        var firstTwoSwapped = Arithmetic.Add(
            Arithmetic.Associator(first, second, third), Arithmetic.Associator(second, first, third));
        var lastTwoSwapped = Arithmetic.Add(
            Arithmetic.Associator(third, first, second), Arithmetic.Associator(third, second, first));
        return Arithmetic.IsZero(firstTwoSwapped) && Arithmetic.IsZero(lastTwoSwapped);
    }

    private static bool IsNormMultiplicative(int dimension)
    {
        var random = new Random(dimension);

        return Enumerable.Range(0, DivisionSamples).All(_ =>
        {
            var x = RandomElement(random, dimension);
            var y = RandomElement(random, dimension);
            var product = Arithmetic.Multiply(x, y);
            var normProduct = Arithmetic.NormSquared(x) * Arithmetic.NormSquared(y);
            return Arithmetic.NormSquared(product) == normProduct;
        });
    }

    private static long[] RandomElement(Random random, int dimension) =>
        [
            .. Enumerable.Range(0, dimension)
                .Select(_ => (long)random.Next(-CoordinateBound, CoordinateBound + 1)),
        ];

    private static IEnumerable<int> BasisIndices(int dimension) => Enumerable.Range(0, dimension);

    private static IEnumerable<(long[] X, long[] Y)> BasisPairs(int dimension) =>
        BasisIndices(dimension).SelectMany(first =>
            BasisIndices(dimension).Select(second =>
                (Arithmetic.Basis(dimension, first), Arithmetic.Basis(dimension, second))));

    private static IEnumerable<(long[] X, long[] Y, long[] Z)> BasisTriples(int dimension) =>
        BasisPairs(dimension).SelectMany(pair =>
            BasisIndices(dimension).Select(third => (pair.X, pair.Y, Arithmetic.Basis(dimension, third))));
}
