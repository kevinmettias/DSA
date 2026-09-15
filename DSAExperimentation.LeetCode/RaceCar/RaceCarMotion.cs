namespace DSAExperimentation.LeetCode.RaceCar;

// The fixed facts about LC 818's car that every arm of the search agrees on: a run
// starts at the origin already moving forward at speed 1, and an 'A' command
// doubles the current speed.
//
// Owned here rather than in RaceCarStateSpace because RaceCarSolution and
// RaceCarStateGraph name them too - they describe the car's motion, of which the
// state space is only one reading.
internal static class RaceCarMotion
{
    public const int StartPosition = 0;
    public const int StartSpeed = 1;
    public const int SpeedDoublingFactor = 2;
}
