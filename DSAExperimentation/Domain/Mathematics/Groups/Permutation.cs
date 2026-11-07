using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace DSAExperimentation.Domain.Mathematics.Groups;

// A permutation of the points 0 to n - 1: a rearrangement that sends each point to one point and no
// two points to the same one, held as its images in one-line notation, images[i] = p(i). n is its
// degree. It is an immutable value: every operation makes a new permutation, and the images are
// never handed out to be written. A permutation of degree n is an element of the symmetric group
// S_n. The group, whose identity needs n fixed per type, is SymmetricGroup<TDegree>; this type
// works at a degree chosen at run time, and default(Permutation) is the permutation of no points.
// What follows from a permutation's cycles, its sign and order among them, is PermutationCycles'.
//
// Composition is written as functions compose: p.After(q) is p ∘ q, which applies q first and then
// p, (p ∘ q)(i) = p(q(i)). Applying a permutation to a sequence moves the item at position i to
// position p(i), so applying q and then p moves it to p(q(i)): it is applying p.After(q).
//
// Every image array is made here and never escapes, so wrapping it as an ImmutableArray without a
// copy keeps the value immutable and costs nothing.
/// <summary>A permutation of the points 0 to n - 1, as an immutable value.</summary>
public readonly struct Permutation : IEquatable<Permutation>
{
    private const string NotAPermutation = "The images do not name each point once.";
    private const string NotDisjointCycles = "A point is in two cycles or past the degree.";
    private const string DifferentDegrees = "The permutations act on different numbers of points.";
    private const string DifferentLength = "The sequence's length is not the permutation's degree.";

    // The images in one-line notation; default(Permutation) holds none, read as no points.
    private readonly ImmutableArray<int> _images;

    /// <summary>Gets how many points the permutation acts on.</summary>
    public int Degree => Images.Length;

    /// <summary>Gets the images in one-line notation: the point each point is sent to.</summary>
    public ReadOnlySpan<int> Images => _images.AsSpan();

    private Permutation(int[] images) =>
        _images = ImmutableCollectionsMarshal.AsImmutableArray(images);

    /// <summary>Makes the identity of a degree, which sends every point to itself.</summary>
    /// <param name="degree">How many points it acts on, 0 or more.</param>
    /// <returns>The identity.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="degree"/> is negative.
    /// </exception>
    public static Permutation Identity(int degree) => new(IdentityImages(degree));

    /// <summary>Reads a permutation from its images in one-line notation.</summary>
    /// <param name="images">The point each point is sent to, each of 0 to n - 1 once.</param>
    /// <returns>The permutation, of degree the images' length.</returns>
    /// <exception cref="ArgumentException">The images do not name each point once.</exception>
    public static Permutation FromImages(ReadOnlySpan<int> images)
    {
        var named = new bool[images.Length];

        foreach (var image in images)
        {
            var isNewPoint = (uint)image < (uint)images.Length && !named[image];

            if (!isNewPoint)
            {
                throw new ArgumentException(NotAPermutation, nameof(images));
            }

            named[image] = true;
        }

        return new(images.ToArray());
    }

    /// <summary>
    /// Reads a permutation from cycle notation: each cycle sends each of its points to the next,
    /// and its last to its first.
    /// </summary>
    /// <param name="degree">How many points the permutation acts on.</param>
    /// <param name="cycles">
    /// Disjoint cycles, each a list of distinct points; a point no cycle names is fixed.
    /// </param>
    /// <returns>The permutation.</returns>
    /// <exception cref="ArgumentException">
    /// A point is named twice or is not below <paramref name="degree"/>.
    /// </exception>
    public static Permutation FromCycles(int degree, params ReadOnlySpan<int[]> cycles)
    {
        var images = IdentityImages(degree);
        var named = new bool[degree];

        foreach (var cycle in cycles)
        {
            for (var i = 0; i < cycle.Length; i++)
            {
                var point = cycle[i];
                var isNewPoint = (uint)point < (uint)degree && !named[point];

                if (!isNewPoint)
                {
                    throw new ArgumentException(NotDisjointCycles, nameof(cycles));
                }

                named[point] = true;
                images[point] = cycle[(i + 1) % cycle.Length];
            }
        }

        return new(images);
    }

    // Each point its own image: the identity's one-line notation, in a new array.
    private static int[] IdentityImages(int degree)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(degree);
        var images = new int[degree];

        for (var point = 0; point < degree; point++)
        {
            images[point] = point;
        }

        return images;
    }

    /// <summary>Finds the point a point is sent to.</summary>
    /// <param name="point">The point, from 0 below <see cref="Degree"/>.</param>
    /// <returns>Its image.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The point is not below the degree.</exception>
    public int Apply(int point)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
            (uint)point, (uint)Degree, nameof(point));
        return Images[point];
    }

    /// <summary>Rearranges a sequence: the item at position i moves to position p(i).</summary>
    /// <typeparam name="Item">The items' type.</typeparam>
    /// <param name="items">The sequence, as long as the degree.</param>
    /// <returns>A new array of the items rearranged.</returns>
    /// <exception cref="ArgumentException">The sequence's length is not the degree.</exception>
    public Item[] Apply<Item>(ReadOnlySpan<Item> items)
    {
        var images = Images;

        if (items.Length != images.Length)
        {
            throw new ArgumentException(DifferentLength, nameof(items));
        }

        var moved = new Item[items.Length];

        for (var position = 0; position < items.Length; position++)
        {
            moved[images[position]] = items[position];
        }

        return moved;
    }

    /// <summary>
    /// Composes this permutation after another: the result applies <paramref name="inner"/> first,
    /// then this one, p ∘ q in function notation.
    /// </summary>
    /// <param name="inner">The permutation applied first, of the same degree.</param>
    /// <returns>The composite, which sends i to this permutation's image of inner(i).</returns>
    /// <exception cref="ArgumentException">The degrees differ.</exception>
    public Permutation After(Permutation inner)
    {
        var outer = Images;
        var first = inner.Images;

        if (first.Length != outer.Length)
        {
            throw new ArgumentException(DifferentDegrees, nameof(inner));
        }

        var composite = new int[outer.Length];

        for (var point = 0; point < composite.Length; point++)
        {
            composite[point] = outer[first[point]];
        }

        return new(composite);
    }

    /// <summary>Makes the inverse, which sends each image back to its point.</summary>
    /// <returns>The inverse, which composed with this one either way gives the identity.</returns>
    public Permutation Inverse()
    {
        var images = Images;
        var inverse = new int[images.Length];

        for (var point = 0; point < images.Length; point++)
        {
            inverse[images[point]] = point;
        }

        return new(inverse);
    }

    /// <summary>Tells whether two permutations are the same: the same degree and images.</summary>
    /// <param name="other">The other permutation.</param>
    /// <returns><see langword="true"/> when they send every point to the same point.</returns>
    public bool Equals(Permutation other) => Images.SequenceEqual(other.Images);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Permutation other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.AddBytes(MemoryMarshal.AsBytes(Images));
        return hash.ToHashCode();
    }
}
