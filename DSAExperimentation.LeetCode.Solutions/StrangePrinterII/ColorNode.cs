namespace DSAExperimentation.LeetCode.Solutions.StrangePrinterII;

// Edges point earlier-printed color -> later-printed color: a color's bounding
// rectangle always prints as one solid block first, so any different color found
// inside that rectangle was necessarily printed on top of it afterward - the same
// "prerequisite -> dependent" shape LeetCode/CourseSchedule's CourseNode uses.
//
// Answers LC 1591 alone. Its shape is exactly DataStructures' AdjacencyNode, a color
// for the id and MustPrintBefore for the out-edges, which ARCHITECTURE.md §13.3 has a
// problem use through a `using` alias; it has not moved onto one yet. It previously
// existed twice: once as a Tests fixture and once again as a private copy inside
// StrangePrinterIIBenchmarks; this is the single surviving declaration.
internal sealed class ColorNode(int color)
{
    public int Color { get; } = color;

    public List<ColorNode> MustPrintBefore { get; } = [];

    public override string ToString() => Color.ToString();
}
