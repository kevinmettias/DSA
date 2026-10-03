namespace DSAExperimentation.Benchmarks.Fixtures;

// Seeded arrays whose product is planted to fit in a 32-bit int, for the problems that
// promise one: LC 238 that every answer[i] fits, LC 1352 that the product of the stream
// does at every point. A uniform draw cannot keep that promise past a few dozen values -
// thirty-one factors of 2 already overflow - so every value is 1 except a few planted
// factors in [2, factorBoundExclusive), placed at seeded positions until the next one
// would push their product past int.MaxValue. Every factor is at least 2, so there are at
// most 30 of them, however long the array.
//
// A Random is taken rather than a seed for the reason SeededDraws gives: a caller that
// draws more after this keeps one stream.
internal static class BoundedProductDraws
{
    private const int MinFactor = 2;

    // A value's sign, drawn with equal odds.
    private static readonly int[] Signs = [-1, 1];

    // count positive values whose product is at most int.MaxValue.
    public static int[] Factors(int count, int factorBoundExclusive, Random random)
    {
        var values = new int[count];
        Array.Fill(values, 1);

        var positions = SeededSequences.ShuffledZeroTo(count, random);
        var planted = 0;
        var product = 1L;
        var factor = random.Next(MinFactor, factorBoundExclusive);

        while (planted < count && product * factor <= int.MaxValue)
        {
            values[positions[planted]] = factor;
            product *= factor;
            planted++;
            factor = random.Next(MinFactor, factorBoundExclusive);
        }

        return values;
    }

    // Factors, each then given a random sign: negating a value leaves the product's
    // magnitude, and so the int bound, where it was.
    public static int[] SignedFactors(int count, int magnitudeBoundExclusive, Random random)
    {
        var values = Factors(count, magnitudeBoundExclusive, random);

        for (var i = 0; i < values.Length; i++)
        {
            values[i] *= Signs[random.Next(Signs.Length)];
        }

        return values;
    }
}
