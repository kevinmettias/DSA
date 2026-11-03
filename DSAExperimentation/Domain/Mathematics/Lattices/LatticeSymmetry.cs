using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Mathematics.Lattices;

// What a planar lattice's symmetries do to a step: turn it some number of the lattice's smallest
// rotations, or list every step a symmetry maps it to. A set of steps that agrees with the lattice is
// a union of such orbits: on the square lattice the four orthogonal steps are one, the four
// diagonals another, and the eight L-shaped jumps a third.
//
// An orbit is listed in the order it is generated: the step and its turns, then its mirror image and
// that one's turns, each step once. That order is fixed but means nothing. A table whose order is
// part of a contract keeps its own literal order (GridDirections), and tests check it against the
// orbit as a set.
/// <summary>Turns a lattice step, and lists every step the lattice's symmetries map it to.</summary>
public static class LatticeSymmetry
{
    /// <summary>Turns a step clockwise by a number of the lattice's smallest rotations; a negative number turns it anticlockwise.</summary>
    /// <typeparam name="TStep">The lattice's step type.</typeparam>
    /// <param name="step">The step to turn.</param>
    /// <param name="turns">How many of the lattice's smallest rotations to turn it by, clockwise.</param>
    /// <returns>The step turned.</returns>
    public static TStep Turn<TStep>(TStep step, int turns)
        where TStep : struct, IPlanarLatticeStep<TStep>
    {
        var clockwiseTurns = turns % TStep.RotationOrder;

        if (clockwiseTurns < 0)
        {
            clockwiseTurns += TStep.RotationOrder;
        }

        for (var turn = 0; turn < clockwiseTurns; turn++)
        {
            step = TStep.Rotate(step);
        }

        return step;
    }

    /// <summary>Lists every step the lattice's rotations and reflections map a step to, each once.</summary>
    /// <typeparam name="TStep">The lattice's step type.</typeparam>
    /// <param name="step">The step whose images to list.</param>
    /// <returns>The step and its turns, then its mirror image and that one's turns, without repeats.</returns>
    public static ImmutableArray<TStep> Orbit<TStep>(TStep step)
        where TStep : struct, IPlanarLatticeStep<TStep>
    {
        var images = ImmutableArray.CreateBuilder<TStep>();
        AddTurns(images, step);
        AddTurns(images, TStep.Reflect(step));

        return images.ToImmutable();
    }

    private static void AddTurns<TStep>(ImmutableArray<TStep>.Builder images, TStep step)
        where TStep : struct, IPlanarLatticeStep<TStep>
    {
        for (var turn = 0; turn < TStep.RotationOrder; turn++)
        {
            var isNew = !images.Contains(step);

            if (isNew)
            {
                images.Add(step);
            }

            step = TStep.Rotate(step);
        }
    }
}
