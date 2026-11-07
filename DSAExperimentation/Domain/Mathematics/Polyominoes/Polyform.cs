using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes;

// A polyform: a finite set of cells on a planar lattice, connected through the steps its cells
// are joined by (IPolyformCells), such as a polyomino on the square lattice or a polyhex on the
// hexagonal one. It is held normalized: its cells sorted in the cells' order, and translated so
// that the first sits at the origin. Two sets of cells that are translates of each other normalize
// to the same list, so equality means the same fixed polyform, and the first cell is where a
// placement is anchored: no other placement of the same image has its first cell on that spot.
//
// A cell is its step from the origin, so the lattice's symmetries act on cells as they act on
// steps, about the origin cell: LatticeSymmetry turns a step, and the step type mirrors one.
// Normalized again, an image is the piece turned or turned over. Which images count as the same
// piece is an equivalence's (fixed, one-sided, free). The piece's orientations are its distinct
// images under the equivalence's symmetries, and by the orbit-stabilizer theorem there are as many
// as the equivalence's order divided by how many of its symmetries map the piece onto itself: 1,
// 2, 4 or 8 free orientations on the square lattice. Polyforms are ordered by size and then cell
// by cell, and a piece's least orientation stands for its class.
//
// It is an immutable value: turning, mirroring and listing orientations return new polyforms. The
// default value is the empty polyform, of no cells.
/// <summary>A connected set of lattice cells, held translated to the origin.</summary>
/// <typeparam name="TStep">The lattice's step type; a cell is its step from the origin.</typeparam>
/// <typeparam name="TCells">
/// How cells are joined and ordered, such as <see cref="SquareCells"/>.
/// </typeparam>
public readonly record struct Polyform<TStep, TCells> : IComparable<Polyform<TStep, TCells>>
    where TStep : struct, IPlanarLatticeStep<TStep>
    where TCells : struct, IPolyformCells<TStep>
{
    private const string RepeatedCell = "A cell is listed twice.";
    private const string DisconnectedCells = "The cells are not connected.";

    private readonly ImmutableArray<TStep> _cells;

    /// <summary>Gets the cells, in the cells' order, the first at the origin.</summary>
    public ImmutableArray<TStep> Cells => _cells.IsDefault ? ImmutableArray<TStep>.Empty : _cells;

    /// <summary>Gets how many cells there are.</summary>
    public int Size => Cells.Length;

    private Polyform(ImmutableArray<TStep> normalized) => _cells = normalized;

    /// <summary>Makes a polyform of a set of cells, wherever they lie.</summary>
    /// <param name="cells">The cells, in any order, each once, joined into one piece.</param>
    /// <returns>The polyform, translated so its first cell is at the origin.</returns>
    /// <exception cref="ArgumentException">
    /// A cell is listed twice, or the cells are not connected.
    /// </exception>
    public static Polyform<TStep, TCells> Of(ReadOnlySpan<TStep> cells)
    {
        var sorted = Sorted(cells);

        if (HasRepeat(sorted))
        {
            throw new ArgumentException(RepeatedCell, nameof(cells));
        }

        if (!IsConnected(sorted))
        {
            throw new ArgumentException(DisconnectedCells, nameof(cells));
        }

        return Translated(sorted);
    }

    // Cells in the cells' order, wherever they lie.
    private static TStep[] Sorted(ReadOnlySpan<TStep> cells)
    {
        var sorted = cells.ToArray();
        Array.Sort(sorted, TCells.Compare);

        return sorted;
    }

    // A sorted list with a cell listed twice holds the two side by side.
    private static bool HasRepeat(TStep[] sorted)
    {
        for (var cell = 1; cell < sorted.Length; cell++)
        {
            if (sorted[cell].Equals(sorted[cell - 1]))
            {
                return true;
            }
        }

        return false;
    }

    // No cells are one piece, none at all; otherwise the first must reach every other.
    private static bool IsConnected(TStep[] sorted) =>
        sorted.Length == 0 || CountReached(sorted) == sorted.Length;

    // How many cells the first reaches, itself among them, through cells joined one to the next.
    private static int CountReached(TStep[] sorted)
    {
        var unreached = new HashSet<TStep>(sorted);
        var frontier = new Stack<TStep>();
        unreached.Remove(sorted[0]);
        frontier.Push(sorted[0]);
        var reached = 1;

        while (frontier.TryPop(out var cell))
        {
            foreach (var step in TCells.JoiningSteps)
            {
                var joined = cell + step;

                if (unreached.Remove(joined))
                {
                    frontier.Push(joined);
                    reached++;
                }
            }
        }

        return reached;
    }

    /// <summary>Turns the polyform by a number of the lattice's smallest rotations.</summary>
    /// <param name="turns">
    /// How many smallest rotations to turn it by, clockwise; a negative number turns it
    /// anticlockwise.
    /// </param>
    /// <returns>The polyform turned, translated so its first cell is at the origin.</returns>
    public Polyform<TStep, TCells> Turned(int turns)
    {
        var cells = Cells;
        var images = new TStep[cells.Length];

        for (var cell = 0; cell < cells.Length; cell++)
        {
            images[cell] = LatticeSymmetry.Turn(cells[cell], turns);
        }

        return Normalized(images);
    }

    /// <summary>Mirrors the polyform in the lattice's line of symmetry, turning it over.</summary>
    /// <returns>The mirror image, translated so its first cell is at the origin.</returns>
    public Polyform<TStep, TCells> Mirrored()
    {
        var cells = Cells;
        var images = new TStep[cells.Length];

        for (var cell = 0; cell < cells.Length; cell++)
        {
            images[cell] = TStep.Reflect(cells[cell]);
        }

        return Normalized(images);
    }

    /// <summary>Lists the polyform's distinct images under an equivalence's symmetries.</summary>
    /// <typeparam name="TEquivalence">Which turns and sides count as the same piece.</typeparam>
    /// <returns>
    /// The polyform and its turns, then its mirror image and that one's turns, each image once.
    /// </returns>
    public ImmutableArray<Polyform<TStep, TCells>> Orientations<TEquivalence>()
        where TEquivalence : struct, IPolyformEquivalence
    {
        var images = ImmutableArray.CreateBuilder<Polyform<TStep, TCells>>();

        foreach (var image in Images<TEquivalence>())
        {
            var isNew = !images.Contains(image);

            if (isNew)
            {
                images.Add(image);
            }
        }

        return images.ToImmutable();
    }

    /// <summary>Counts the equivalence's symmetries that map the polyform onto itself.</summary>
    /// <typeparam name="TEquivalence">Which turns and sides count as the same piece.</typeparam>
    /// <returns>The order of the polyform's own symmetry group within the equivalence's.</returns>
    public int SymmetryCount<TEquivalence>()
        where TEquivalence : struct, IPolyformEquivalence
    {
        var count = 0;

        foreach (var image in Images<TEquivalence>())
        {
            if (image == this)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Counts the polyform's orientations from its symmetry group.</summary>
    /// <typeparam name="TEquivalence">Which turns and sides count as the same piece.</typeparam>
    /// <returns>
    /// The equivalence's order divided by the polyform's symmetry count, by the orbit-stabilizer
    /// theorem.
    /// </returns>
    public int OrientationCount<TEquivalence>()
        where TEquivalence : struct, IPolyformEquivalence =>
        TEquivalence.Sides * TEquivalence.TurnsOf<TStep>() / SymmetryCount<TEquivalence>();

    /// <summary>Finds the least of the polyform's orientations, to stand for its class.</summary>
    /// <typeparam name="TEquivalence">Which turns and sides count as the same piece.</typeparam>
    /// <returns>The orientation that comes first in polyform order.</returns>
    public Polyform<TStep, TCells> Canonical<TEquivalence>()
        where TEquivalence : struct, IPolyformEquivalence
    {
        var least = this;

        foreach (var image in Images<TEquivalence>())
        {
            if (image.CompareTo(least) < 0)
            {
                least = image;
            }
        }

        return least;
    }

    /// <summary>Whether another polyform is one of this one's orientations.</summary>
    /// <typeparam name="TEquivalence">Which turns and sides count as the same piece.</typeparam>
    /// <param name="other">The other polyform.</param>
    /// <returns>
    /// <see langword="true"/> when the two are the same piece under the equivalence.
    /// </returns>
    public bool IsEquivalentTo<TEquivalence>(Polyform<TStep, TCells> other)
        where TEquivalence : struct, IPolyformEquivalence =>
        Orientations<TEquivalence>().Contains(other);

    /// <summary>Whether two polyforms are the same fixed polyform.</summary>
    /// <param name="other">The other polyform.</param>
    /// <returns><see langword="true"/> when their normalized cells are equal.</returns>
    public bool Equals(Polyform<TStep, TCells> other) =>
        Cells.AsSpan().SequenceEqual(other.Cells.AsSpan());

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var cell in Cells)
        {
            hash.Add(cell);
        }

        return hash.ToHashCode();
    }

    /// <summary>Orders polyforms by size, and then cell by cell in the cells' order.</summary>
    /// <param name="other">The other polyform.</param>
    /// <returns>Negative when this one comes first, zero when equal, else positive.</returns>
    public int CompareTo(Polyform<TStep, TCells> other)
    {
        var cells = Cells;
        var otherCells = other.Cells;
        var order = cells.Length.CompareTo(otherCells.Length);

        for (var cell = 0; order == 0 && cell < cells.Length; cell++)
        {
            order = TCells.Compare(cells[cell], otherCells[cell]);
        }

        return order;
    }

    // Every image under the equivalence's symmetries, repeats and all: each side up in turn, and
    // each of the turns of that side.
    private IEnumerable<Polyform<TStep, TCells>> Images<TEquivalence>()
        where TEquivalence : struct, IPolyformEquivalence
    {
        var side = this;

        for (var sideUp = 0; sideUp < TEquivalence.Sides; sideUp++)
        {
            var image = side;

            for (var turn = 0; turn < TEquivalence.TurnsOf<TStep>(); turn++)
            {
                yield return image;
                image = image.Turned(1);
            }

            side = side.Mirrored();
        }
    }

    // A set of cells the lattice's symmetries moved, sorted and translated back to the origin.
    private static Polyform<TStep, TCells> Normalized(TStep[] images)
    {
        Array.Sort(images, TCells.Compare);

        return Translated(images);
    }

    // Sorted cells moved so the first is at the origin; the cells' order survives a translation, so
    // they stay sorted.
    private static Polyform<TStep, TCells> Translated(TStep[] sorted)
    {
        if (sorted.Length == 0)
        {
            return default;
        }

        var back = -sorted[0];

        for (var cell = 0; cell < sorted.Length; cell++)
        {
            sorted[cell] += back;
        }

        return new Polyform<TStep, TCells>(ImmutableArray.Create(sorted));
    }
}
