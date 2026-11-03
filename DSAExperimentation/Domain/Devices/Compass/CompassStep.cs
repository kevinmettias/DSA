using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Devices.Compass;

// One unit step on the plane, given as how far it goes east and how far north, because the problems
// disagree on which axis is which (CompassRules). The four steps a compass heading takes are named,
// so a reader sees Northward rather than decoding (0, 1). A positional record already names its
// parts East and North, so the steps are named for where they go: Northward, Eastward.
//
// Each named step is the square lattice's own unit step (GridStep) read with north up: the grid's
// rows grow downward, so a step's northward part is its rows negated and its eastward part its
// columns, and Northward is GridStep.Up. The compass and the grid hold one fact between them, the
// four unit steps a quarter turn apart (the owner's ruling, 2026-11-03).
/// <summary>A step on the plane: how far it goes east, and how far north.</summary>
/// <param name="East">How far the step goes east; negative goes west.</param>
/// <param name="North">How far the step goes north; negative goes south.</param>
public readonly record struct CompassStep(int East, int North)
{
    /// <summary>One unit north: (0, 1).</summary>
    public static CompassStep Northward { get; } = From(GridStep.Up);

    /// <summary>One unit east: (1, 0).</summary>
    public static CompassStep Eastward { get; } = From(GridStep.Right);

    /// <summary>One unit south: (0, -1).</summary>
    public static CompassStep Southward { get; } = From(GridStep.Down);

    /// <summary>One unit west: (-1, 0).</summary>
    public static CompassStep Westward { get; } = From(GridStep.Left);

    /// <summary>Reads a step of the grid with north up: up the rows is north, and right along the columns is east.</summary>
    /// <param name="step">The grid step, as rows down and columns right.</param>
    /// <returns>The same step, as how far it goes east and how far north.</returns>
    public static CompassStep From(GridStep step) => new(step.DeltaCol, -step.DeltaRow);
}
