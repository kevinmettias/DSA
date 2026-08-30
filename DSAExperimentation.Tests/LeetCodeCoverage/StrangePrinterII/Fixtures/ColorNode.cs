namespace DSAExperimentation.Tests.LeetCodeCoverage.StrangePrinterII.Fixtures;

// Edges point earlier-printed color -> later-printed color: a color's bounding
// rectangle always prints as one solid block first, so any different color found
// inside that rectangle was necessarily printed on top of it afterward - the same
// "prerequisite -> dependent" shape CourseSchedule's CourseNode already uses.
internal sealed class ColorNode(int color)
{
    public int Color { get; } = color;

    public List<ColorNode> MustPrintBefore { get; } = [];

    public override string ToString() => Color.ToString();
}
