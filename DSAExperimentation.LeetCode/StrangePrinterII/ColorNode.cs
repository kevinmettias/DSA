namespace DSAExperimentation.LeetCode.StrangePrinterII;

// Edges point earlier-printed color -> later-printed color: a color's bounding
// rectangle always prints as one solid block first, so any different color found
// inside that rectangle was necessarily printed on top of it afterward - the same
// "prerequisite -> dependent" shape LeetCode/CourseSchedule's CourseNode uses.
//
// Answers LC 1591 alone - a plain adjacency-list graph node with no fixed vertex
// set or modulus - so it lives beside the solution rather than in Domain/ or
// DataStructures/ (ARCHITECTURE.md §17.3/§17.6). It previously existed twice: once
// as a Tests fixture and once again as a private copy inside
// StrangePrinterIIBenchmarks; this is the single surviving declaration.
internal sealed class ColorNode(int color)
{
    public int Color { get; } = color;

    public List<ColorNode> MustPrintBefore { get; } = [];

    public override string ToString() => Color.ToString();
}
