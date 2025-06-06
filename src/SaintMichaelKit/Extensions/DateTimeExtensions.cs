namespace SaintMichaelKit.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Checks if the DateTime is a weekend (Saturday or Sunday).
    /// </summary>
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Returns the next weekday after the current date.
    /// </summary>
    public static DateTime NextWeekday(this DateTime date)
    {
        var nextDate = date.AddDays(1);
        while (nextDate.IsWeekend())
        {
            nextDate = nextDate.AddDays(1);
        }
        return nextDate;
    }

    /// <summary>
    /// Checks if the date is the last day of the month.
    /// </summary>
    public static bool IsLastDayOfMonth(this DateTime date)
    {
        return date.Day == DateTime.DaysInMonth(date.Year, date.Month);
    }

    /// <summary>
    /// Returns the start of the day (00:00:00) for the date.
    /// </summary>
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// Returns the end of the day (23:59:59.9999999) for the date.
    /// </summary>
    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Adds business days to the date, skipping Saturdays and Sundays.
    /// </summary>
    public static DateTime AddBusinessDays(this DateTime date, int businessDays)
    {
        if (businessDays == 0)
            return date;

        int direction = businessDays > 0 ? 1 : -1;
        int absDays = Math.Abs(businessDays);

        DateTime currentDate = date;
        while (absDays > 0)
        {
            currentDate = currentDate.AddDays(direction);
            if (!currentDate.IsWeekend())
                absDays--;
        }
        return currentDate;
    }

    /// <summary>
    /// Formats the date in the format dd/MM/yyyy (can be changed as needed).
    /// </summary>
    public static string ToBrazilianDateString(this DateTime date)
    {
        return date.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Returns the first day of the specified month. If not specified, returns the first day of the date's month.
    /// </summary>
    /// <param name="date">Base date for calculation</param>
    /// <param name="year">Optional year</param>
    /// <param name="month">Optional month</param>
    /// <returns>Date representing the first day of the month</returns>
    public static DateTime FirstDayOfMonth(this DateTime date, int? year = null, int? month = null)
    {
        return new DateTime(year ?? date.Year, month ?? date.Month, 1);
    }

    /// <summary>
    /// Returns the last day of the specified month. If not specified, returns the last day of the date's month.
    /// </summary>
    /// <param name="date">Base date for calculation</param>
    /// <param name="year">Optional year</param>
    /// <param name="month">Optional month</param>
    /// <returns>Date representing the last day of the month</returns>
    public static DateTime LastDayOfMonth(this DateTime date, int? year = null, int? month = null)
    {
        return new DateTime(year ?? date.Year, month ?? date.Month, 1)
            .AddMonths(1)
            .AddDays(-1);
    }
}