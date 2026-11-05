namespace DSAExperimentation.Domain.Science.Time.Calendars;

// A date as a calendar writes it: a year, a month of that year and a day of that month. Which
// calendar it is written in is the caller's to know, as it is on paper; Calendar reads it in one.
/// <summary>A date as a year, a month and a day.</summary>
/// <param name="Year">The year, from 1, the first year of the era.</param>
/// <param name="Month">The month's number, January 1 to December 12.</param>
/// <param name="Day">The day of the month, from 1.</param>
public readonly record struct CalendarDate(int Year, int Month, int Day);
