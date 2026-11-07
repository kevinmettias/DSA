using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;

namespace DSAExperimentation.Algorithms.Enumeration;

// Every polyform of n cells, grown from those of n - 1. Each fixed polyform of n - 1 cells with one
// cell joined to it added is a fixed polyform of n cells, and every one of n cells arises so: a
// cell that is a leaf of a spanning tree of its cells can be taken away without disconnecting the
// rest. Each grown set is normalized and kept once, which gives the fixed polyforms, and an
// equivalence's classes are their distinct least orientations. Redelmeier's method (1981) counts
// fixed polyforms faster, without keeping them or comparing normal forms; this one lists them,
// which a set of puzzle pieces needs. Each level is kept in polyform order, so the lists come out
// the same on every run.
/// <summary>Lists every polyform of a size, grown a cell at a time from the smaller ones.</summary>
public static class PolyformGrowth
{
    /// <summary>Lists every fixed polyform of a number of cells.</summary>
    /// <typeparam name="TStep">The lattice's step type.</typeparam>
    /// <typeparam name="TCells">How cells are joined and ordered.</typeparam>
    /// <param name="size">How many cells, at least 1.</param>
    /// <returns>Each fixed polyform once, in polyform order.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="size"/> is less than 1.
    /// </exception>
    public static ImmutableArray<Polyform<TStep, TCells>> Fixed<TStep, TCells>(int size)
        where TStep : struct, IPlanarLatticeStep<TStep>
        where TCells : struct, IPolyformCells<TStep>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        ImmutableArray<Polyform<TStep, TCells>> level =
            [Polyform<TStep, TCells>.Of([TStep.AdditiveIdentity])];

        for (var cells = 1; cells < size; cells++)
        {
            level = Grown(level);
        }

        return level;
    }

    // The fixed polyforms one cell larger: each one's cells with each joined cell not in it added.
    private static ImmutableArray<Polyform<TStep, TCells>> Grown<TStep, TCells>(
        ImmutableArray<Polyform<TStep, TCells>> level)
        where TStep : struct, IPlanarLatticeStep<TStep>
        where TCells : struct, IPolyformCells<TStep>
    {
        var grown = new SortedSet<Polyform<TStep, TCells>>();

        foreach (var form in level)
        {
            AddGrown(grown, form);
        }

        return [.. grown];
    }

    // Each polyform one cell larger than a given one: its cells and one cell joined to them.
    private static void AddGrown<TStep, TCells>(
        SortedSet<Polyform<TStep, TCells>> grown, Polyform<TStep, TCells> form)
        where TStep : struct, IPlanarLatticeStep<TStep>
        where TCells : struct, IPolyformCells<TStep>
    {
        var cells = form.Cells;
        var larger = new TStep[cells.Length + 1];
        cells.CopyTo(larger);
        var members = new HashSet<TStep>(cells);

        foreach (var cell in cells)
        {
            foreach (var step in TCells.JoiningSteps)
            {
                larger[cells.Length] = cell + step;

                if (!members.Contains(larger[cells.Length]))
                {
                    grown.Add(Polyform<TStep, TCells>.Of(larger));
                }
            }
        }
    }

    /// <summary>Lists each class of an equivalence once, as its least orientation.</summary>
    /// <typeparam name="TStep">The lattice's step type.</typeparam>
    /// <typeparam name="TCells">How cells are joined and ordered.</typeparam>
    /// <typeparam name="TEquivalence">Which turns and sides count as the same piece.</typeparam>
    /// <param name="size">How many cells, at least 1.</param>
    /// <returns>Each class's least orientation once, in polyform order.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="size"/> is less than 1.
    /// </exception>
    public static ImmutableArray<Polyform<TStep, TCells>> Classes<TStep, TCells, TEquivalence>(
        int size)
        where TStep : struct, IPlanarLatticeStep<TStep>
        where TCells : struct, IPolyformCells<TStep>
        where TEquivalence : struct, IPolyformEquivalence
    {
        var fixedForms = Fixed<TStep, TCells>(size);
        var classes = new SortedSet<Polyform<TStep, TCells>>();

        foreach (var form in fixedForms)
        {
            classes.Add(form.Canonical<TEquivalence>());
        }

        return [.. classes];
    }
}
