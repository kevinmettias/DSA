using System.Collections.Immutable;
using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Domain.Mathematics.Groups;

// What follows from a permutation's cycles. Following i, p(i), p(p(i)), ... returns to i, and the
// points fall into these cycles, a fixed point a cycle of one. A cycle of k points is k - 1
// transpositions, so with c cycles a permutation of n points is n - c of them, and its sign is
// (-1)^(n - c), a homomorphism onto {+1, -1}: sign(p ∘ q) = sign(p) sign(q). The parity of the
// inversion count is the same parity, but the cycles give it in O(n). The order, the least k > 0
// with p^k the identity, is the lcm of the cycle lengths, since p^k fixes a cycle's points exactly
// when k is a multiple of its length.
//
// The order is a long, kept with checked arithmetic. The largest order among permutations of n
// points is Landau's function g(n), and g(347) is about 8.3e18 while g(348) passes long.MaxValue,
// so Order never throws below degree 348. Above it, a permutation whose order would pass
// long.MaxValue throws instead of wrapping.
/// <summary>A permutation's cycles, and its count of them, sign, parity and order.</summary>
public static class PermutationCycles
{
    /// <summary>The sign of an even permutation, a product of an even number of swaps.</summary>
    public const int EvenSign = 1;

    /// <summary>The sign of an odd permutation, a product of an odd number of swaps.</summary>
    public const int OddSign = -1;

    /// <summary>Counts a permutation's cycles, each fixed point a cycle of one.</summary>
    /// <param name="permutation">The permutation.</param>
    /// <returns>How many cycles its points fall into.</returns>
    public static int Count(Permutation permutation)
    {
        var images = permutation.Images;
        var visited = new bool[images.Length];
        var cycles = 0;

        for (var start = 0; start < visited.Length; start++)
        {
            if (!visited[start])
            {
                WalkCycle(images, start, visited);
                cycles++;
            }
        }

        return cycles;
    }

    /// <summary>Tells whether a permutation is a product of an even number of swaps.</summary>
    /// <param name="permutation">The permutation.</param>
    /// <returns><see langword="true"/> when its degree less its cycle count is even.</returns>
    public static bool IsEven(Permutation permutation) =>
        int.IsEvenInteger(permutation.Degree - Count(permutation));

    /// <summary>Finds a permutation's sign: +1 when it is even, -1 when it is odd.</summary>
    /// <param name="permutation">The permutation.</param>
    /// <returns><see cref="EvenSign"/> or <see cref="OddSign"/>.</returns>
    public static int Sign(Permutation permutation)
    {
        var isEven = IsEven(permutation);
        return isEven ? EvenSign : OddSign;
    }

    /// <summary>
    /// Finds a permutation's order, the fewest copies of it that compose to the identity.
    /// </summary>
    /// <param name="permutation">The permutation.</param>
    /// <returns>The lcm of its cycle lengths, 1 for the identity.</returns>
    /// <exception cref="OverflowException">
    /// The order passes long.MaxValue, which needs degree 348 or more.
    /// </exception>
    public static long Order(Permutation permutation)
    {
        var images = permutation.Images;
        var visited = new bool[images.Length];
        var order = 1L;

        for (var start = 0; start < visited.Length; start++)
        {
            if (!visited[start])
            {
                long length = WalkCycle(images, start, visited);
                order = checked(order / GreatestCommonDivisor.Of(order, length) * length);
            }
        }

        return order;
    }

    /// <summary>
    /// Decomposes a permutation into its cycles, each fixed point a cycle of one: each cycle from
    /// its smallest point, in order of those points.
    /// </summary>
    /// <param name="permutation">The permutation.</param>
    /// <returns>The cycles, every point in exactly one.</returns>
    public static ImmutableArray<ImmutableArray<int>> Of(Permutation permutation)
    {
        var images = permutation.Images;
        var visited = new bool[images.Length];
        var cycles = ImmutableArray.CreateBuilder<ImmutableArray<int>>();

        for (var start = 0; start < visited.Length; start++)
        {
            if (!visited[start])
            {
                var cycle = CycleThrough(images, start, visited);
                cycles.Add(cycle);
            }
        }

        return cycles.ToImmutable();
    }

    // The cycle through start, from start on, each of its points marked as visited.
    private static ImmutableArray<int> CycleThrough(
        ReadOnlySpan<int> images, int start, bool[] visited)
    {
        var length = WalkCycle(images, start, visited);
        var cycle = ImmutableArray.CreateBuilder<int>(length);

        for (var point = start; cycle.Count < length; point = images[point])
        {
            cycle.Add(point);
        }

        return cycle.MoveToImmutable();
    }

    // Marks every point of the cycle through start as visited, and answers how many there are.
    private static int WalkCycle(ReadOnlySpan<int> images, int start, bool[] visited)
    {
        var length = 0;

        for (var point = start; !visited[point]; point = images[point])
        {
            visited[point] = true;
            length++;
        }

        return length;
    }
}
