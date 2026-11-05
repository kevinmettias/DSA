namespace DSAExperimentation.Domain.Science.Time.Calendars;

// A month of the Julian family's calendars as a named value: its number in the year and how many
// days it has in a common year. Only February's length changes, by the leap day a leap year adds to
// it, so a month's own length is the common year's and the calendar adds the day (Calendar). A
// month's name is a language's, not the calendar's.
/// <summary>A month: its number in the year and its days in a common year.</summary>
/// <param name="Number">The month's number, January 1 to December 12.</param>
/// <param name="DaysInCommonYear">The days the month has in a year without a leap day.</param>
public readonly record struct CalendarMonth(int Number, int DaysInCommonYear);
