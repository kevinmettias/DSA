namespace DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

// What PartitionProbe reads off one grid: each count and list is what a partition into n regions of
// n cells makes all ones, all n, or empty.
internal sealed record PartitionFacts(
    int[] CoverCounts,
    (int Row, int Col, int Region)[] CellsListedOutsideTheirRegion,
    int[] RegionSizes,
    (int Placed, int Asked)[] IsInRegionDisagreements);
