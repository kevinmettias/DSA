using DSAExperimentation.Domain.Mathematics.Groups;

namespace DSAExperimentation.Tests.Domain.Mathematics.Groups;

// The cycles of a few named permutations, then the facts every permutation keeps, checked over all
// of S_4 or S_5: the sign is a homomorphism, the parity is the inversion count's, and the order is
// the least power giving the identity. Fixed points are cycles too, so a walk that skipped them
// would miscount the transposition (0 1) of three points, whose parity needs the fixed point 2.
public sealed partial class PermutationCyclesTests
{
    // 2 * 3 * 5 * ... * 47, the order of a permutation with cycles of the primes to 47 as lengths.
    private const long PrimorialOfFortySeven = 614_889_782_588_491_410;

    // The prime after 47: one more cycle of this length takes the order past long.MaxValue.
    private const int NextPrime = 53;

    public static TheoryData<int, int, int> Transpositions =>
        new() { { 2, 0, 1 }, { 3, 0, 1 }, { 5, 1, 3 }, { 7, 6, 0 } };

    public static TheoryData<int> CycleLengths => new() { 1, 2, 3, 4, 5, 6, 7 };

    public static TheoryData<int[]> PrimeCycleLengths =>
        new() { new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 } };

    [Fact]
    public void Count_CountsAFixedPointAsACycle() =>
        Assert.Equal(3, PermutationCycles.Count(Permutation.FromCycles(5, [0, 2, 4])));

    [Fact]
    public void Count_OfTheIdentity_IsTheDegree() =>
        Assert.Equal(6, PermutationCycles.Count(Permutation.Identity(6)));

    [Fact]
    public void Of_ListsEachCycleFromItsSmallestPoint_InOrderOfThosePoints()
    {
        int[][] expected = [[0, 3], [1], [2, 5, 4]];
        var cycles = PermutationCycles.Of(Permutation.FromCycles(6, [4, 2, 5], [3, 0]));

        Assert.Equal(expected, cycles.Select(cycle => cycle.ToArray()));
    }

    [Fact]
    public void Of_ReadBackAsCycleNotation_GivesThePermutation() =>
        Assert.All(AllOfDegree(5), permutation => Assert.Equal(permutation, ReadBack(permutation)));

    [Theory]
    [MemberData(nameof(Transpositions))]
    public void Sign_OfATransposition_IsOdd(int degree, int first, int second) =>
        Assert.Equal(
            PermutationCycles.OddSign,
            PermutationCycles.Sign(Permutation.FromCycles(degree, [first, second])));

    [Theory]
    [MemberData(nameof(CycleLengths))]
    public void Sign_OfACycleOfNPoints_IsMinusOneToTheNMinusOne(int length)
    {
        var cycle = Permutation.FromCycles(length, [.. Enumerable.Range(0, length)]);
        var isOddLength = int.IsOddInteger(length);
        var expected = isOddLength ? PermutationCycles.EvenSign : PermutationCycles.OddSign;

        Assert.Equal(expected, PermutationCycles.Sign(cycle));
    }

    [Fact]
    public void Sign_IsAHomomorphism() =>
        Assert.All(AllOfDegree(4), left =>
            Assert.All(AllOfDegree(4), right => AssertSignMultiplies(left, right)));

    [Fact]
    public void IsEven_IsTheInversionCountsParity() =>
        Assert.All(AllOfDegree(5), permutation =>
            Assert.Equal(
                int.IsEvenInteger(InversionCount(permutation)),
                PermutationCycles.IsEven(permutation)));

    [Fact]
    public void IsEven_OfTheIdentity_IsTrue() =>
        Assert.True(PermutationCycles.IsEven(Permutation.Identity(5)));

    // lcm(4, 2) = 4 where the product of the lengths would say 8.
    [Fact]
    public void Order_IsTheLcmOfTheCycleLengths_NotTheirProduct() =>
        Assert.Equal(4, PermutationCycles.Order(Permutation.FromCycles(6, [0, 1, 2, 3], [4, 5])));

    [Fact]
    public void Order_IsTheLeastPowerGivingTheIdentity() =>
        Assert.All(AllOfDegree(5), permutation =>
            Assert.Equal(
                LeastPowerGivingTheIdentity(permutation), PermutationCycles.Order(permutation)));

    [Theory]
    [MemberData(nameof(PrimeCycleLengths))]
    public void Order_OfCyclesOfDistinctPrimeLengths_IsTheirProduct(int[] lengths) =>
        Assert.Equal(PrimorialOfFortySeven, PermutationCycles.Order(OfCycleLengths(lengths)));

    [Theory]
    [MemberData(nameof(PrimeCycleLengths))]
    public void Order_PastLongMaxValue_Throws(int[] lengths)
    {
        var tooLong = OfCycleLengths([.. lengths, NextPrime]);

        Assert.Throws<OverflowException>(() => PermutationCycles.Order(tooLong));
    }

    // Every permutation of a degree, by placing each point in turn at every free position.
    internal static IEnumerable<Permutation> AllOfDegree(int degree) =>
        Arrangements([], [.. Enumerable.Range(0, degree)])
            .Select(images => Permutation.FromImages(images));

    private static IEnumerable<int[]> Arrangements(int[] placed, int[] unplaced)
    {
        if (unplaced.Length == 0)
        {
            return [placed];
        }

        return unplaced.SelectMany(point =>
            Arrangements([.. placed, point], [.. unplaced.Where(other => other != point)]));
    }

    private static Permutation ReadBack(Permutation permutation)
    {
        int[][] cycles = [.. PermutationCycles.Of(permutation).Select(cycle => cycle.ToArray())];

        return Permutation.FromCycles(permutation.Degree, cycles);
    }

    private static void AssertSignMultiplies(Permutation left, Permutation right)
    {
        var productOfSigns = PermutationCycles.Sign(left) * PermutationCycles.Sign(right);

        Assert.Equal(productOfSigns, PermutationCycles.Sign(left.After(right)));
    }

    // The textbook count, pairs i < j with p(i) > p(j), as the oracle the cycle parity must match.
    private static int InversionCount(Permutation permutation)
    {
        var images = permutation.Images.ToArray();
        var inversions = 0;

        for (var first = 0; first < images.Length; first++)
        {
            inversions += images.Skip(first + 1).Count(image => image < images[first]);
        }

        return inversions;
    }

    private static long LeastPowerGivingTheIdentity(Permutation permutation)
    {
        var identity = Permutation.Identity(permutation.Degree);
        var power = permutation;
        var exponent = 1L;

        while (!power.Equals(identity))
        {
            power = power.After(permutation);
            exponent++;
        }

        return exponent;
    }

    // Consecutive cycles of the given lengths: the first on 0 to lengths[0] - 1, and so on.
    private static Permutation OfCycleLengths(int[] lengths)
    {
        var starts = lengths.Select((_, index) => lengths.Take(index).Sum()).ToArray();
        int[][] cycles =
            [.. lengths.Select((length, index) => Enumerable.Range(starts[index], length))
                .Select(cycle => cycle.ToArray())];

        return Permutation.FromCycles(lengths.Sum(), cycles);
    }
}
